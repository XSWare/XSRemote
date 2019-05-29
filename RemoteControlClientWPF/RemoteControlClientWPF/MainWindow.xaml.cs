using System.Net;
using System.Windows;
using XSLibrary.Network.Connections;

namespace RemoteControlClientWPF
{
    public partial class MainWindow : Window
    {
        TCPPacketConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            OpenLogin();
        }

        private void OpenLogin()
        {
            Login login = new Login();
            login.SuccessfullyConnected += OnLogin;
            Content = login;
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
