using RemoteServer.User;
using System.Net.Sockets;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    abstract class Registration
    {
        TCPAccepter m_accepter;
        protected Logger Logger { get; private set; }

        public Registration(TCPAccepter accepter)
        {
            m_accepter = accepter;

            Logger = new LoggerConsole();
            m_accepter.Logger = Logger;
            m_accepter.ClientConnected += OnClientConnected;
            m_accepter.Run();

            Logger.Log("Registration is now accepting connections on port " + m_accepter.Port);
        }

        void OnClientConnected(object sender, Socket acceptedSocket)
        {
            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);

            connection.Logger = Logger;
            if (!connection.InitializeCrypto(new ECCrypto(false)))
                return;

            HandleVerifiedConnection(DummyDataBase.Instance.GetAccount("dummy"), connection);
        }

        protected abstract void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection);
    }
}
