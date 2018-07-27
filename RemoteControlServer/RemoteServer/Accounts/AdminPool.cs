using XSLibrary.Network.Registrations;

namespace RemoteServer.Accounts
{
    class AdminPool : IAccountPool<AdminAccount>
    {
        public AdminPool() : base(0) { }

        protected override AdminAccount CreateElement(string ID)
        {
            return new AdminAccount(ID) { Logger = Logger };
        }
    }
}
