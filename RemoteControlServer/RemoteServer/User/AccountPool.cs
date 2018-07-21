using XSLibrary.Network.Registrations;

namespace RemoteServer.User
{
    class AccountPool : IAccountPool<UserAccount>
    {
        protected override UserAccount CreateAccount(string username)
        {
            return new UserAccount(username);
        }
    }
}
