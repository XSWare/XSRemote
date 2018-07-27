using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class AdminAccount : IUserAccount
    {
        IConnection Connection { get; set; }
        MultiLogger ParentLog { get; set; }

        public AdminAccount(string adminname, MultiLogger parentLog) : base(adminname)
        {
            ParentLog = parentLog;
        }

        public void SetConnection (IConnection connection)
        {
            Connection = connection;
            NetworkLogger logger = new NetworkLogger() { Connection = connection };
            ParentLog.Logs.Add(logger);
            connection.OnDisconnect += (sender, remote) => ParentLog.Logs.Remove(logger);
        }

        public override void Dispose()
        {
        }
    }
}
