using RemoteServer.User;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.ThreadSafety.Executors;
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    abstract class Registration : IDisposable
    {
        delegate void DisposeHandler();
        event DisposeHandler OnDispose;

        private Logger logger = Logger.NoLog;
        public Logger Logger
        {
            get { return logger; }
            set
            {
                logger = value;
                m_accepter.Logger = value;
            }
        }

        TCPAccepter m_accepter;
        static protected FileUserBase DataBase { get; private set; } = new FileUserBase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\", "accounts.txt");
        static public SafeList<UserAccount> Accounts { get; private set; } = new SafeList<UserAccount>();
        public int AuthenticationTimeout { get; set; } = 30000;

        protected SafeExecutor DataBaseLock { get; private set; } = new SingleThreadExecutor();
        SafeExecutor m_memoryLock = new SingleThreadExecutor();

        public Registration(TCPAccepter accepter)
        {
            m_accepter = accepter;
        }

        public void Run()
        {
            m_accepter.ClientConnected += OnClientConnected;
            m_accepter.Run();

            Logger.Log(LogLevel.Warning, "Registration is now accepting connections on port {0}.", m_accepter.Port);
        }

        void OnClientConnected(object sender, Socket acceptedSocket)
        {
            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);
#if DEBUG
            connection.Logger = Logger;
#endif

            connection.HandshakeTimeout = 30000;
            if(!connection.InitializeCrypto(new RSALegacyCrypto(false)))
                return;
            
            if (!Authenticate(out UserAccount user, connection))
            {
                Logger.Log(LogLevel.Error, "Authentication failed.");
                connection.Disconnect();
                return;
            }

            OnDispose += connection.Dispose;
            connection.OnDisconnect += DisconnectHandler;

            m_memoryLock.Execute(() =>
            {
                if (!Accounts.Contains(user))
                    Accounts.Add(user);
                HandleVerifiedConnection(user, connection);
            });
        }

        bool Authenticate(out UserAccount user, IConnection connection)
        {
            user = null;

            try
            {
                int timeoutBuffer = connection.ReceiveTimeout;
                connection.ReceiveTimeout = AuthenticationTimeout;
                if (!connection.Receive(out byte[] userData, out EndPoint source))
                    return false;

                connection.ReceiveTimeout = timeoutBuffer;

                string userString = Encoding.ASCII.GetString(userData);
                string[] userSplit = userString.Split(' ');

                if (userSplit.Length != 2)
                    return false;

                string username = userSplit[0];

                if (!DataBaseLock.Execute(() => DataBase.Validate(username, Encoding.ASCII.GetBytes(userSplit[1]))))
                    return false;

                connection.Send(new byte[1] { (byte)'+' }, 30);

                user = GetUserAccount(username);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public UserAccount GetUserAccount(string username)
        {
            foreach (UserAccount account in Accounts.Entries)
            {
                if (account.Username == username)
                    return account;
            }

            UserAccount user = new UserAccount(username);
            user.Logger = Logger;
            user.OnConnectionRemoval += HandleConnectionRemoval;

            Logger.Log(LogLevel.Detail, "Allocated memory for account \"{0}\".", user.Username);

            return user;
        }

        private void HandleConnectionRemoval(object sender)
        {
            m_memoryLock.Execute(() =>
            {
                UserAccount user = sender as UserAccount;
                if (!user.StillInUse())
                {
                    Accounts.Remove(user);
                    Logger.Log(LogLevel.Detail, "Memory of account \"{0}\" released.", user.Username);
                }
            });
        }

        protected abstract void HandleVerifiedConnection(UserAccount user, IConnection clientConnection);

        private void DisconnectHandler(object sender, EndPoint remote)
        {
            IConnection connection = sender as IConnection;
            if (connection != null)
                OnDispose -= connection.Dispose;
        }

        public void Dispose()
        {
            m_accepter.Dispose();
            OnDispose?.Invoke();
        }
    }
}
