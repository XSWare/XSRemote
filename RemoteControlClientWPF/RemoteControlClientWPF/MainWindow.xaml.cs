using System.Net;
using System.Windows;
using XSLibrary.Network.Connections;

namespace RemoteControlClientWPF
{
    public partial class MainWindow : Window
    {
        Login m_login;

        TCPPacketConnection Connection { get; set; }
        

        public MainWindow()
        {
            InitializeComponent();
            m_login = new Login();
            m_login.SuccessfullyConnected += OnLogin;

            
            OpenLogin();

            if (m_login.AutoLogin)
                m_login.Connect();

        }

        private void OpenLogin()
        {
            Content = m_login;
        }

        private void OnLogin(object sender, TCPPacketConnection connection)
        {
            Content = new Control(connection);
            Connection = connection;
            connection.OnDisconnect.Event += OnDisconnect;
        }

        private void OnDisconnect(object sender, EndPoint remote)
        {
            OpenLogin();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TCPPacketConnection connection = Connection;
            if(connection != null)
            {
                Connection = null;
                connection.OnDisconnect.Event -= OnDisconnect;
                connection.Disconnect();
            }
        }
    }
}
