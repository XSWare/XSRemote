using System.Net;
using System.Windows;
using System.Windows.Controls;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.Utility;

namespace RemoteControlClientWPF
{
    public partial class Login : UserControl
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
                m_textblock.Text = text;
            }
        }

        public delegate void SuccessfullyConnectedHandler(object sender, TCPPacketConnection connection);
        public event SuccessfullyConnectedHandler SuccessfullyConnected;

        AccountConnector m_connector = new AccountConnector();
        TextblockLogger Logger { get; set; }

        string Server
        {
            get { return m_txtServer.Text; }
            set { m_txtServer.Text = value; }
        }

        string Username
        {
            get { return m_txtUser.Text; }
            set { m_txtUser.Text = value; }
        }

        string Password
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

        private void Connect()
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

            if (m_connector.Connect(remote, out connection))
            {
                ClearStatus();
                connection.OnDisconnect.Event += (object sender, EndPoint source) => SetStatus("Disconnected.");
                if (SuccessfullyConnected != null)
                {
                    SuccessfullyConnected(this, connection);
                }
                else
                    connection.Disconnect();
            }
        }

        private void SetStatus(string text)
        {
            m_status.Text = text;
        }

        private void ClearStatus()
        {
            m_status.Text = "";
        }
    }
}
