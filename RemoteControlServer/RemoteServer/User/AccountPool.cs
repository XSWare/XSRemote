using System;
using XSLibrary.ThreadSafety.MemoryPool;

namespace RemoteServer.User
{
    class AccountPool : IMemoryPool<string, UserAccount>
    {
        protected override UserAccount CreateElement(string username, Action<string> referenceCallback)
        {
            return new UserAccount(username, referenceCallback);
        }
    }
}
