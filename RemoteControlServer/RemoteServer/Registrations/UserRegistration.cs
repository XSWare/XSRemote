using RemoteServer.User;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Accepters;
using RemoteServer.Connections;
using XSLibrary.ThreadSafety.Containers;
using System.Net;
using System.Text;

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

        public void AddUser(string userName, string password)
        {
            DataBase.AddAccount(userName, Encoding.ASCII.GetBytes(password));
        }

        protected override void HandleVerifiedConnection(UserAccount user, IConnection connection)
        {
            UserConnection userConnection = new UserConnection(connection, user);

            user.SetUserConnection(userConnection);
            userConnection.OnDisconnect += OnUserDisconnect;
            m_userConnections.Add(userConnection);
            connection.InitializeReceiving();
        }

        private void OnUserDisconnect(object sender, EndPoint remote)
        {
            m_userConnections.Remove(sender as UserConnection);
        }
    }
}
