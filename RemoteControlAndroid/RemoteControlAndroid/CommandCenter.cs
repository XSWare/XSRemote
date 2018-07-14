using RemoteShutdowLibrary;
using RemoteShutdownLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;

namespace RemoteControlAndroid
{
    class CommandCenter
    {
        public static event IConnection.CommunicationErrorHandler OnDisconnect;

        public static CommandCenter Instance { get; private set; } = new CommandCenter();

        public static bool Connected { get { return Instance.m_connection != null && Instance.m_connection.Connected; } }
        public string LastError { get; private set; } = "";
        public int KeepAliveInterval { get; private set; } = 10000;

        TCPPacketConnection m_connection = null;

        private CommandCenter()
        {
            new Thread(KeepAliveLoop).Start();
        }

        public void Connect(EndPoint remote, Action callback)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try { socket.Connect(remote); }
            catch
            {
                LastError = "Failed to connect.";
                callback();
                return;
            }

            if (m_connection != null)
                m_connection.OnDisconnect -= HandleDisconnect;

            m_connection = new TCPPacketConnection(socket);
            if (!m_connection.InitializeCrypto(new RSALegacyCrypto(true)))
            {
                LastError = "Handshake failed.";
                callback();
                return;
            }

            m_connection.OnDisconnect += HandleDisconnect;
            m_connection.InitializeReceiving();
            m_connection.Send(Encoding.ASCII.GetBytes("dave gina2000"));
            callback();
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

        private void KeepAliveLoop()
        {
            while (true)
            {
                Thread.Sleep(KeepAliveInterval);

                if(Connected)
                    Instance.m_connection.SendKeepAlive();
            }
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            OnDisconnect?.Invoke(this, remote);
        }
    }
}