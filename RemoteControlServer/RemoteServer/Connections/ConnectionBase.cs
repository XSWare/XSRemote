using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    abstract class ConnectionBase
    {
        public event OnDisconnectEvent.EventHandle OnDisconnect
        {
            add { m_connection.OnDisconnect += value; }
            remove { m_connection.OnDisconnect -= value; }
        }

        public event DataReceivedHandler OnDataReceived;

        public delegate void DataReceivedHandler(object sender, string data);

        protected IConnection m_connection;

        public Logger Logger { get; set; }

        public ConnectionBase(IConnection connection)
        {
            Logger = new LoggerConsole();

            m_connection = connection;

            m_connection.DataReceivedEvent += ReceiveData;
        }

        public virtual void Send(string command)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(command));
        }

        private void ReceiveData(object sender, byte[] data, EndPoint source)
        {
            OnDataReceived(this, TransmissionConverter.ConvertByteToString(data));
        }
    }
}
