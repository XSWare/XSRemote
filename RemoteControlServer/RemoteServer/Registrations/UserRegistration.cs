using RemoteServer.User;
using XSLibrary.Network.Connections;
using RemoteServer.Connections;
using XSLibrary.ThreadSafety.Containers;
using System.Net;
using System.Text;
using XSLibrary.Utility;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;

namespace RemoteServer.Registrations
{
    class UserRegistration : Registration
    {
        SafeList<UserConnection> m_userConnections;

        public UserConnection[] UserConnections { get { return m_userConnections.Entries; } }

        public UserRegistration(TCPAcceptor accepter, IUserDataBase dataBase, AccountPool accounts)
            : base(accepter, dataBase, accounts)
        {
            m_userConnections = new SafeList<UserConnection>();
        }

        public void AddUser(string username, string password)
        {
            if (DataBase.AddAccount(username, Encoding.ASCII.GetBytes(password)))
                Logger.Log(LogLevel.Priority, "Added user \"{0}\" to database.", username);
        }

        public void DeleteUser(string username)
        {
            if(DataBase.EraseAccount(username))
                Logger.Log(LogLevel.Priority, "Removed user \"{0}\" from database.", username);
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if(DataBase.ChangePassword(username, Encoding.ASCII.GetBytes(oldPassword), Encoding.ASCII.GetBytes(newPassword)))
                Logger.Log(LogLevel.Priority, "Changed password for user \"{0}\".", username);
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
