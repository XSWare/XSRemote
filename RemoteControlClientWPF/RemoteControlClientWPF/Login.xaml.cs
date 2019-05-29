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

        public Login()
        {
            InitializeComponent();
            Logger = new TextblockLogger(m_status);
            m_connector.Logger = Logger;
            m_connector.Crypto = CryptoType.RSALegacy;
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

            SetStatus("Connecting...");

            EndPoint remote = new IPEndPoint(ip, 80);

            TCPPacketConnection connection;
            m_connector.Login = Username + " " + Password;

            new Thread(() =>
            {
                if (m_connector.Connect(remote, out connection))
                {
                    ClearStatus();
                    connection.OnDisconnect.Event += (object sender, EndPoint source) => SetStatus("Disconnected.");
                    if (SuccessfullyConnected != null)
                    {
                        Dispatcher.Invoke(() => SuccessfullyConnected(this, connection));
                    }
                    else
                        connection.Disconnect();
                }
            }).Start();
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
