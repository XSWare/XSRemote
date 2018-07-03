using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    abstract class ConnectionBase
    {
        public delegate void ClientDisconnectHandler(object sender);
        public event ClientDisconnectHandler OnDisconnect;

        protected TCPConnection m_connection;

        public Logger Logger { get; set; }

        public ConnectionBase(TCPConnection connection)
        {
            Logger = new LoggerConsole();

            m_connection = connection;
            m_connection.DataReceivedEvent += ReceiveData;
            m_connection.ReceiveErrorEvent += OnConnectionLoss;
            m_connection.InitializeReceiving();
        }

        public virtual void SendCommand(string command)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(command));
        }

        protected abstract void ReceiveCommand(string command);

        private void ReceiveData(object sender, byte[] data)
        {
            ReceiveCommand(TransmissionConverter.ConvertByteToString(data));
        }

        protected void OnConnectionLoss(object sender, IPEndPoint endPoint)
        {
            ClientDisconnectHandler handler = OnDisconnect;
            if (handler != null)
                handler.Invoke(this);
        }

    }
}
