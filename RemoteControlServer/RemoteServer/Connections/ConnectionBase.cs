using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    abstract class ConnectionBase
    {
        public delegate void ClientDisconnectHandler(object sender);
        public event ClientDisconnectHandler OnDisconnect;

        protected TCPPacketConnection m_connection;

        public Logger Logger { get; set; }

        public ConnectionBase(TCPPacketConnection connection)
        {
            Logger = new LoggerConsole();

            m_connection = connection;

            m_connection.DataReceivedEvent += ReceiveData;
            m_connection.OnDisconnect += OnConnectionLoss;
            m_connection.InitializeReceiving();
        }

        public virtual void Send(string command)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(command));
        }

        protected abstract void ReceiveCommand(string command);

        private void ReceiveData(object sender, byte[] data, IPEndPoint source)
        {
            ReceiveCommand(TransmissionConverter.ConvertByteToString(data));
        }

        protected void OnConnectionLoss(object sender, IPEndPoint endPoint)
        {
            OnDisconnect?.Invoke(this);
        }

    }
}
