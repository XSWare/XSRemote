using XSLibrary.Network.Registrations;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class AdminPool : IAccountPool<AdminAccount>
    {
        CommandQueue AccountCommands { get; set; }
        MultiLogger ParentLog { get; set; }

        public AdminPool(CommandQueue accountCommands, MultiLogger parentLog) : base(0)
        {
            AccountCommands = accountCommands;
            ParentLog = parentLog;
        }

        protected override AdminAccount CreateElement(string ID)
        {
            return new AdminAccount(ID, AccountCommands, ParentLog) { Logger = Logger };
        }
    }
}
