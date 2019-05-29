using System.Net;
using System.Windows;
using XSLibrary.Network.Connections;

namespace RemoteControlClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            connection.OnDisconnect.Event += OnDisconnect;
        }

        private void OnDisconnect(object sender, EndPoint remote)
        {
            OpenLogin();
        }
    }
}
