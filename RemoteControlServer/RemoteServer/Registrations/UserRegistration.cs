using RemoteServer.User;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Accepters;
using RemoteServer.Connections;
using XSLibrary.ThreadSafety.Containers;

namespace RemoteServer.Registrations
{
    class UserRegistration : Registration
    {
        SafeList<UserConnection> m_userConnections;

        public UserConnection[] UserConnections { get { return m_userConnections.Entries; } }

        public UserRegistration(TCPAccepter accepter)
            : base(accepter)
        {
            m_userConnections = new SafeList<UserConnection>();
        }

        protected override void HandleVerifiedConnection(UserAccount user, IConnection connection)
        {
            UserConnection userConnection = new UserConnection(connection, user);
            if (!userConnection.Initialize())
                return;

            user.SetUserConnection(userConnection);
            userConnection.OnDisconnect += OnUserDisconnect;
            m_userConnections.Add(userConnection);
        }

        private void OnUserDisconnect(object sender)
        {
            m_userConnections.Remove(sender as UserConnection);
        }
    }
}
