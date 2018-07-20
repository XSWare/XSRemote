using RemoteShutdowLibrary;
using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.Connections;

namespace RemoteControlAndroid
{
    class CommandCenter
    {
        public static event OnDisconnectEvent.EventHandle OnDisconnect;
        public static event IConnection.DataReceivedHandler OnDataReceived;

        public static CommandCenter Instance { get; private set; } = new CommandCenter();

        public static bool Connected { get { return Instance.m_connection != null && Instance.m_connection.Connected; } }
        public int KeepAliveInterval { get; private set; } = 10000;

        TCPPacketConnection m_connection = null;

        private CommandCenter()
        {
        }

        public static void SetConnection(TCPPacketConnection connection)
        {
            Instance.m_connection = connection;
            connection.DataReceivedEvent += Instance.HandleDataReceived;
            connection.InitializeReceiving();
            connection.OnDisconnect += Instance.HandleDisconnect;
        }

        public static void SendControlCommand(string cmd)
        {
            Instance.SendCommand(Commands.CONTROL + " " + cmd);
        }

        public static void SendMediaCommand(string cmd)
        {
            Instance.SendCommand(Commands.MEDIA + " " + cmd);
        }

        public static void SendVolumeCommand(string cmd)
        {
            Instance.SendCommand(Commands.VOLUME + " " + cmd);
        }

        private void SendCommand(string cmd)
        {
            if (!Connected)
                return;

            m_connection.Send(TransmissionConverter.ConvertStringToByte(cmd));
        }

        public static void Disconnect()
        {
            if (Connected)
                Instance.m_connection.Disconnect();
        }

        public static void SendKeepAlive()
        {
            if (Connected)
                Instance.m_connection.SendKeepAlive();
        }

        private void HandleDataReceived(object sender, byte[] data, EndPoint source)
        {
            OnDataReceived?.Invoke(this, data, source);
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            //if(!Reconnect())
                OnDisconnect?.Invoke(this, remote);
        }

        //private bool Reconnect()
        //{
        //    if (m_lastLogin == null)
        //        return false;

        //    Connect(m_lastLogin, () => { });
        //    return Connected;
        //}
    }
}