using RemoteServer.User;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Accepters;
using RemoteServer.KeyExchanges;
using RemoteServer.Authentications;
using RemoteServer.Connections;
using XSLibrary.ThreadSafety.Containers;

namespace RemoteServer.Registrations
{
    class UserRegistration : Registration
    {
        SafeList<UserConnection> m_userConnections;

        public UserConnection[] UserConnections { get { return m_userConnections.Entries; } }

        public UserRegistration(TCPAccepter accepter, KeyExchange keyExchange, Authentication authentication)
            : base(accepter, keyExchange, authentication)
        {
            m_userConnections = new SafeList<UserConnection>();
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection)
        {
            UserConnection userConnection = new UserConnection(clientConnection, user);
            userConnection.OnDisconnect += OnUserDisconnect;
            m_userConnections.Add(new UserConnection(clientConnection, user));
            clientConnection.Logger.Log("User \"{0}\" logged in.", user.UserData.Username);
        }

        private void OnUserDisconnect(object sender)
        {
            m_userConnections.Remove(sender as UserConnection);
        }
    }
}
