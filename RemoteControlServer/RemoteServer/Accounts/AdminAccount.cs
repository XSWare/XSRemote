using XSLibrary.Network.Registrations;

namespace RemoteServer.Accounts
{
    class AdminAccount : IUserAccount
    {
        public AdminAccount(string adminname) : base(adminname)
        {

        }

        public override void Dispose()
        {
        }
    }
}
