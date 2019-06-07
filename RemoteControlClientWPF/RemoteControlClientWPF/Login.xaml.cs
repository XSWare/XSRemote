using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.Utility;

namespace RemoteControlClientWPF
{
    class TextblockLogger : Logger
    {
        TextBlock m_textblock;

        public TextblockLogger(TextBlock textBlock)
        {
            m_textblock = textBlock;
        }

        protected override void LogMessage(string text)
        {
            m_textblock.Dispatcher.Invoke(() => m_textblock.Text = text);
        }
    }

    public partial class Login : UserControl
    {
        public delegate void SuccessfullyConnectedHandler(object sender, TCPPacketConnection connection);
        public event SuccessfullyConnectedHandler SuccessfullyConnected;

        public delegate void LoginFailedHandler(object sender);
        public event LoginFailedHandler LoginFailed;

        AccountConnector m_connector = new AccountConnector();
        TextblockLogger Logger { get; set; }

        AppConfiguration Configuration { get; set; } = new AppConfiguration();

        bool m_connecting = false;
        bool Connecting
        {
            get { return m_connecting; }
            set
            {
                m_connecting = value;
                SetReadonly(m_connecting);
            }
        }

        public string Server
        {
            get { return m_txtServer.Text; }
            set { m_txtServer.Text = value; }
        }

        public string Username
        {
            get { return m_txtUser.Text; }
            set { m_txtUser.Text = value; }
        }

        public string Password
        {
            get { return m_txtPassword.Password; }
            set { m_txtPassword.Password = value; }
        }

        public bool StorePassword
        {
            get { return (bool)m_btnStorePassword.IsChecked; }
            set { m_btnStorePassword.IsChecked = value; }
        }

        public bool AutoLogin
        {
            get { return IsAutoLoginEnabled() && (bool)m_btnAutoLogin.IsChecked; }
            set { m_btnAutoLogin.IsChecked = value; }
        }

        public Login()
        {
            InitializeComponent();
            Logger = new TextblockLogger(m_status);
            Logger.LogLevel = LogLevel.Information;
            m_connector.Logger = Logger;
            m_connector.Crypto = CryptoType.EC25519;

            ApplyAutoLoginReadonly();
        }

        public void SetConfiguration(AppConfiguration config)
        {
            Configuration = config;

            Server = Configuration.Server;
            Username = Configuration.User;

            if (Configuration.StorePassword)
                Password = Configuration.Password;

            StorePassword = Configuration.StorePassword;
            AutoLogin = Configuration.AutoLogin;

            ApplyAutoLoginReadonly();
        }

        public void SetFocus()
        {
            if (m_txtServer.Text.Length == 0)
                m_txtServer.Focus();
            else if (m_txtUser.Text.Length == 0)
                m_txtUser.Focus();
            else if (m_txtPassword.Password.Length == 0)
                m_txtPassword.Focus();
            else
                m_btnConnect.Focus();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        public void Connect()
        {
            // obviously not threadsafe so don't call it from  different threads
            if (Connecting)
                return;

            Connecting = true;

            int port = 22222;
            string ipString = Server;

            int portStart = Server.IndexOf(':');
            if (portStart >= 0)
            {
                ipString = Server.Substring(0, portStart);
                try { port = Convert.ToInt32(Server.Substring(portStart + 1)); }
                catch
                {
                    Logger.Log(LogLevel.Priority, "Invalid port.");
                    Connecting = false;
                    return;
                }
            }

            IPAddress ip;
            if (!IPAddress.TryParse(ipString, out ip))
            {
                Logger.Log(LogLevel.Priority, "Invalid server/IP.");
                Connecting = false;
                return;
            }

            SetStatus("Connecting...");

            EndPoint remote = new IPEndPoint(ip, port);

            TCPPacketConnection connection;
            m_connector.Login = Username + " " + Password;

            new Thread(() =>
            {
                if (m_connector.Connect(remote, out connection))
                {
                    SetStatus("Connected.");
                    SaveConfig();
                    connection.OnDisconnect.Event += (object sender, EndPoint source) => SetStatus("Disconnected.");

                    if (SuccessfullyConnected != null)
                        Dispatcher.Invoke(() => SuccessfullyConnected(this, connection));
                    else
                        connection.Disconnect();
                }
                else
                    Dispatcher.Invoke(() => LoginFailed?.Invoke(this));

                Connecting = false;
            }).Start();
        }

        private void SaveConfig()
        {
            Dispatcher.Invoke(() =>
            {
                Configuration.Server = Server;
                Configuration.User = Username;
                Configuration.StorePassword = StorePassword;

                if (Configuration.StorePassword)
                    Configuration.Password = Password;
                else
                    Configuration.Password = "";

                Configuration.AutoLogin = AutoLogin;

                Configuration.StoreConfig();
            });
        }

        private void SetReadonly(bool readOnly)
        {
            Dispatcher.Invoke(() =>
            {
                m_txtServer.IsEnabled = !readOnly;
                m_txtUser.IsEnabled = !readOnly;
                m_txtPassword.IsEnabled = !readOnly;
                m_btnStorePassword.IsEnabled = !readOnly;
                m_btnAutoLogin.IsEnabled = !readOnly && IsAutoLoginEnabled();
                m_btnConnect.IsEnabled = !readOnly;
            });
        }

        private void ApplyAutoLoginReadonly()
        {
            m_btnAutoLogin.IsEnabled = IsAutoLoginEnabled();
        }

        private bool IsAutoLoginEnabled()
        {
            return StorePassword;
        }

        private void SetStatus(string text)
        {
            Dispatcher.Invoke(() => m_status.Text = text);
        }

        private void ClearStatus()
        {
            SetStatus("");
        }

        private void OnStorePasswordClicked(object sender, RoutedEventArgs e)
        {
            ApplyAutoLoginReadonly();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Connect();
        }

        private void OnTextFieldGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void OnPasswordFieldGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox textBox = sender as PasswordBox;
            textBox.SelectAll();
        }
    }
}
