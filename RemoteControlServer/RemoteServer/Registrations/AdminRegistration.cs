using RemoteServer.Accounts;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    class AdminRegistration : Registration<AdminAccount>
    {
        MultiLogger parentLogger { get; set; }

        public AdminRegistration(MultiLogger logger, TCPAcceptor accepter, AdminPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts, dataBase)
        {
            parentLogger = logger;
        }

        protected override void HandleVerifiedConnection(AdminAccount user, TCPPacketConnection clientConnection)
        {
            user.SetConnection(clientConnection);
        }
    }
}
