using System;
using XSLibrary.ThreadSafety.MemoryPool;

namespace RemoteServer.User
{
    class AccountPool : IMemoryPool<string, UserAccount>
    {
        protected override UserAccount CreateElement(string username, Action<string> referenceCallback)
        {
            UserAccount newUser = new UserAccount(username, referenceCallback);
            newUser.Logger = Logger;
            return newUser;
        }
    }
}
