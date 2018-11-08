using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.Connections;
using XSLibrary.ThreadSafety.Events;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    abstract class ConnectionBase
    {
        public IEvent<ConnectionBase, EndPoint> OnDisconnect;

        public event DataReceivedHandler OnDataReceived;

        public delegate void DataReceivedHandler(object sender, string data);

        protected IConnection m_connection;

        public Logger Logger { get; set; }

        public ConnectionBase(IConnection connection)
        {
            Logger = new LoggerConsole();

            m_connection = connection;

            m_connection.DataReceivedEvent += ReceiveData;
            OnDisconnect = m_connection.OnDisconnect.CreateRelay(this);
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
