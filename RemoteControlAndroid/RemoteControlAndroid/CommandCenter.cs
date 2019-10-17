using RemoteShutdownLibrary;
using System;
using System.Net;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.ThreadSafety.Events;
using XSLibrary.Utility;

namespace RemoteControlAndroid
{
    class CommandCenter
    {
        public delegate void ConnectHandle();

        public static event ConnectHandle OnConnect;
        public static OneShotEvent<CommandCenter, EndPoint> OnDisconnect = new OneShotEvent<CommandCenter, EndPoint>();
        public static event IConnection.DataReceivedHandler OnDataReceived;

        public static CommandCenter Instance { get; private set; } = new CommandCenter();

        public static bool Connected { get { return Instance.m_connection != null && Instance.m_connection.Connected; } }
        public static bool CurrentlyConnecting { get { return Instance.m_connector.CurrentlyConnecting; } }
        public static bool DisconnectedGracefully { get { return Instance.m_disconnectedGracefully; } }
        public int KeepAliveTime { get; private set; } = 10000;
        public int KeepAliveInterval { get; private set; } = 1000;

        Logger m_logger = Logger.NoLog;
        public static Logger ActiveLogger
        {
            get { return Instance.m_logger; }
            set
            {
                Instance.m_logger = value;
                Instance.m_connector.Logger = Instance.m_logger;
            }
        }

        AccountConnector m_connector;
        TCPPacketConnection m_connection = null;

        bool m_disconnectedGracefully = false;

        private CommandCenter()
        {
            m_connector = CreateConnector();
        }

        private AccountConnector CreateConnector()
        {
            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.EC25519;
            connector.TimeoutCryptoHandshake = 30000;
            

            return connector;
        }

        public static void Connect(EndPoint remote, string username, string password)
        {
            Instance.m_disconnectedGracefully = false;
            Instance.m_connector.Login = username + " " + password;

            Action<TCPPacketConnection> successCallback = ((connection) =>
            {
                Instance.SetConnection(connection);
                OnConnect?.Invoke();
            });

            Instance.m_connector.ConnectAsync(remote, successCallback, () => { });
        }

        void SetConnection(TCPPacketConnection connection)
        {
            Instance.m_connection = connection;
            OnDisconnect = new OneShotEvent<CommandCenter, EndPoint>();
            connection.DataReceivedEvent += Instance.HandleDataReceived;
            connection.InitializeReceiving();
            connection.OnDisconnect.Event += Instance.HandleDisconnect;
            connection.SetUpKeepAlive(KeepAliveTime, KeepAliveInterval);
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
            Instance.m_disconnectedGracefully = true;

            if (Connected)
                Instance.m_connection.Disconnect();
        }

        private void HandleDataReceived(object sender, byte[] data, EndPoint source)
        {
            OnDataReceived?.Invoke(this, data, source);
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            if (!DisconnectedGracefully && !Reconnect())
            {
                OnDisconnect.Invoke(this, remote);
                ActiveLogger.Log(LogLevel.Warning, "Disconnected.");
            }
        }

        private bool Reconnect()
        {
            Instance.m_disconnectedGracefully = false;

            if (!m_connector.Reconnect(out TCPPacketConnection connection))
                return false;

            SetConnection(connection);
            OnConnect?.Invoke();

            return Connected;
        }
    }
}