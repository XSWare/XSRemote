using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;
using XSLibrary.ThreadSafety.Executors;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class AdminAccount : IUserAccount
    {
        MultiLogger ParentLog { get; set; }
        IConnection Connection { get; set; } = null;
        NetworkLogger NetworkLog { get; set; } = null;
        SafeExecutor m_lock = new SingleThreadExecutor();

        public AdminAccount(string adminname, MultiLogger parentLog) : base(adminname)
        {
            ParentLog = parentLog;
        }

        public bool SetConnection (IConnection connection)
        {
            return m_lock.Execute(() =>
            {
                if (Connection != null)
                {
                    Logger.Log(LogLevel.Priority, "Admin tried to connect to account \"{0}\" but it already has an admin connection.", Username);
                    return false;
                }

                Logger.Log(LogLevel.Priority, "User connected to account \"{0}\".", Username);

                Connection = connection;
                connection.Logger = Logger.NoLog;   // must not log the connection which is used for logging
                NetworkLog = new NetworkLogger() { Connection = connection };
                ParentLog.Logs.Add(NetworkLog);
                return true;
            });
        }

        public void OnAdminDisconnect(object sender, System.Net.EndPoint arguments)
        {
            m_lock.Execute(() =>
            {
                Connection = null;

                if (ParentLog.Logs.Remove(NetworkLog))
                    NetworkLog.Dispose();

                Logger.Log(LogLevel.Priority, "Admin disconnected from account \"{0}\".", Username);
            });
        }

        public override void Dispose()
        {

        }
    }
}
