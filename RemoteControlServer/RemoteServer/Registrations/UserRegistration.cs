using RemoteServer.User;
using XSLibrary.Network.Connections;
using RemoteServer.Connections;
using XSLibrary.ThreadSafety.Containers;
using System.Net;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;

namespace RemoteServer.Registrations
{
    class UserRegistration : Registration
    {
        SafeList<UserConnection> m_userConnections;

        public UserConnection[] UserConnections { get { return m_userConnections.Entries; } }

        public UserRegistration(TCPAcceptor accepter, UserPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts, dataBase)
        {
            m_userConnections = new SafeList<UserConnection>();
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection)
        {
            UserConnection userConnection = new UserConnection(clientConnection, user);

            user.SetUserConnection(userConnection);
            userConnection.OnDisconnect += OnUserDisconnect;
            m_userConnections.Add(userConnection);
            clientConnection.InitializeReceiving();
        }

        private void OnUserDisconnect(object sender, EndPoint remote)
        {
            m_userConnections.Remove(sender as UserConnection);
        }
    }
}
