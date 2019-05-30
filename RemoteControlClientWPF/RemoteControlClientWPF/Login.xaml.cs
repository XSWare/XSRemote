using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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

        AccountConnector m_connector = new AccountConnector();
        TextblockLogger Logger { get; set; }

        LoginConfiguration Configuration { get; set; } = new LoginConfiguration();

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
            get { return m_txtPassword.Text; }
            set { m_txtPassword.Text = value; }
        }

        public bool StorePassword
        {
            get { return (bool)m_btnStorePassword.IsChecked; }
            set { m_btnStorePassword.IsChecked = value; }
        }

        public bool AutoLogin
        {
            get { return (bool)m_btnAutoLogin.IsChecked; }
            set { m_btnAutoLogin.IsChecked = value; }
        }

        public Login()
        {
            InitializeComponent();
            Logger = new TextblockLogger(m_status);
            Logger.LogLevel = LogLevel.Information;
            m_connector.Logger = Logger;
            m_connector.Crypto = CryptoType.RSALegacy;

            Configuration.LoadConfig();
            Server = Configuration.Server;
            Username = Configuration.User;

            if (Configuration.StorePassword)
                Password = Configuration.Password;
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        public void Connect()
        {
            IPAddress ip;
            if (!IPAddress.TryParse(Server, out ip))
            {
                Logger.Log(LogLevel.Priority, "Invalid server/IP.");
                return;
            }

            SetReadonly(true);

            SetStatus("Connecting...");

            EndPoint remote = new IPEndPoint(ip, 80);

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

                SetReadonly(false);
            }).Start();
        }

        private void SaveConfig()
        {
            Configuration.Server = Server;
            Configuration.User = Username;
            Configuration.StorePassword = (bool)m_btnStorePassword.IsChecked;

            if (Configuration.StorePassword)
                Configuration.Password = Password;
            else
                Configuration.Password = "";

            Configuration.AutoLogin = (bool)m_btnAutoLogin.IsChecked;

            Configuration.StoreConfig();
        }

        private void SetReadonly(bool readOnly)
        {

        }

        private void SetStatus(string text)
        {
            Dispatcher.Invoke(() => m_status.Text = text);
        }

        private void ClearStatus()
        {
            SetStatus("");
        }
    }
}
