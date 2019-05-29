using System.Windows;
using System.Windows.Controls;
using XSLibrary.Network.Connections;

namespace RemoteControlClientWPF
{
    public partial class Control : UserControl
    {
        TCPPacketConnection Connection { get; set; }
        public Control(TCPPacketConnection connection)
        {
            Connection = connection;
            InitializeComponent();
        }

        private void OnDisconnectClick(object sender, RoutedEventArgs e)
        {
            Connection.Disconnect();
        }
    }
}
