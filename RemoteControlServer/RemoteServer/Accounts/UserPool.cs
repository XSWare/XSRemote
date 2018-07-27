using XSLibrary.Network.Registrations;

namespace RemoteServer.Accounts
{
    class UserPool : IAccountPool<UserAccount>
    {
        public UserPool() : base(5) { }

        protected override UserAccount CreateElement(string username)
        {
            return new UserAccount(username) { Logger = Logger };
        }
    }
}
