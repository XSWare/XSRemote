using XSLibrary.Network.Registrations;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class AdminPool : IAccountPool<AdminAccount>
    {
        MultiLogger ParentLog { get; set; }

        public AdminPool(MultiLogger parentLog) : base(0)
        {
            ParentLog = parentLog;
        }

        protected override AdminAccount CreateElement(string ID)
        {
            return new AdminAccount(ID, ParentLog) { Logger = Logger };
        }
    }
}
