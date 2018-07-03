using RemoteServer.User;

namespace RemoteServer
{
    class DummyDataBase
    {
        static DummyDataBase m_instance = new DummyDataBase();

        public static DummyDataBase Instance { get { return m_instance; } }

        UserAccount m_dummyAccount;

        private DummyDataBase()
        {
            m_dummyAccount = new UserAccount(new UserData("dummy", new byte[0], new byte[0]));
        }

        public UserAccount GetAccount(string userName)
        {
            return m_dummyAccount;
        }
    }
}
