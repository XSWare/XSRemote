using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;
using XSLibrary.ThreadSafety.Executors;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class AdminAccount : IUserAccount
    {
        MultiLogger ParentLog { get; set; }
        CommandQueue AccountCommands { get; set; }
        IConnection Connection { get; set; } = null;
        NetworkLogger NetworkLog { get; set; } = null;
        SafeExecutor m_lock = new SingleThreadExecutor();

        public AdminAccount(string adminname, CommandQueue accountCommands, MultiLogger parentLog) : base(adminname)
        {
            AccountCommands = accountCommands;
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

                Logger.Log(LogLevel.Priority, "Admin connected to account \"{0}\".", Username);

                Connection = connection;
                connection.Logger = Logger.NoLog;   // must not log the connection which is used for logging
                NetworkLog = new NetworkLogger() { Connection = connection };
                ParentLog.Logs.Add(NetworkLog);
                return true;
            });
        }

        public void OnAdminDisconnect(object sender, System.Net.EndPoint arguments)
        {
            Disconnect();
        }

        public void OnCommandReceived(object sender, byte[] data, EndPoint source)
        {
            string reply = TransmissionConverter.ConvertByteToString(data);

            Logger.Log(LogLevel.Information, "Received command \"{0}\" from admin \"{1}\".", reply, Username);
            AccountCommands.SendMessage(reply);
        }

        public override void Dispose()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            m_lock.Execute(() =>
            {
                if (Connection != null)
                {
                    Connection.OnDisconnect.Event -= OnAdminDisconnect;
                    Connection.Disconnect();
                }

                Connection = null;

                if (ParentLog.Logs.Remove(NetworkLog))
                    NetworkLog.Dispose();

                Logger.Log(LogLevel.Priority, "Admin disconnected from account \"{0}\".", Username);
            });
        }
    }
}
