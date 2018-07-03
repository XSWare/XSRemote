using RemoteServer.User;
using System.Collections.Generic;

namespace RemoteServer
{
    class DummyDataBase
    {
        static DummyDataBase m_instance = new DummyDataBase();

        public static DummyDataBase Instance { get { return m_instance; } }

        List<UserAccount> m_userAccounts = new List<UserAccount>();
        UserAccount m_dummyAccount;

        private DummyDataBase()
        {
            AddAccount((new UserData("dummy", new byte[0], new byte[0])));
            m_dummyAccount = m_userAccounts[0];
        }

        public void AddAccount(UserData userData)
        {
            m_userAccounts.Add(new UserAccount(userData));
        }

        public UserAccount GetAccount(string userName)
        {
            return m_dummyAccount;
        }
    }
}
