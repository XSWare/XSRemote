using XSLibrary.ThreadSafety.MemoryPool;

namespace RemoteServer.User
{
    class AccountPool : IMemoryPool<string, UserAccount>
    {
        protected override UserAccount CreateElement(string username)
        {
            return new UserAccount(username) { Logger = Logger };
        }
    }
}
