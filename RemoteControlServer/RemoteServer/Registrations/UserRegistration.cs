using RemoteServer.Accounts;
using XSLibrary.Network.Connections;
using RemoteServer.Connections;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;

namespace RemoteServer.Registrations
{
    class UserRegistration : Registration<UserAccount>
    {
        public UserRegistration(TCPAcceptor accepter, UserPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts, dataBase)
        {
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection)
        {
            UserConnection userConnection = new UserConnection(clientConnection, user);

            if (user.SetUserConnection(userConnection))
            {
                clientConnection.InitializeReceiving();
                clientConnection.OnDisconnect.Event += user.HandleUserDisconnect;
            }
            else
                clientConnection.Disconnect();
        }
    }
}
