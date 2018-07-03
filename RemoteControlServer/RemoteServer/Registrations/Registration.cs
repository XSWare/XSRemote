using RemoteServer.Authentications;
using RemoteServer.KeyExchanges;
using RemoteServer.User;
using System.Net.Sockets;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    abstract class Registration
    {
        TCPAccepter m_accepter;
        KeyExchange m_keyExchange;
        Authentication m_authentication;

        public Registration(TCPAccepter accepter, KeyExchange keyExchange, Authentication authentication)
        {
            m_accepter = accepter;
            m_keyExchange = keyExchange;
            m_authentication = authentication;

            m_accepter.Logger = new LoggerConsole();
            m_accepter.ClientConnected += OnClientConnected;
            m_accepter.Run();

            m_accepter.Logger.Log("Registration is now accepting connections on port " + m_accepter.Port);
        }

        void OnClientConnected(object sender, Socket acceptedSocket)
        {
            if (!m_keyExchange.DoKeyExchange(acceptedSocket))
                return;

            if (!m_authentication.DoAuthentication(acceptedSocket, out UserAccount user))
                return;

            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);
            //connection.Logger = new LoggerConsole();
            HandleVerifiedConnection(user, connection);
        }

        protected abstract void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection);
    }
}
