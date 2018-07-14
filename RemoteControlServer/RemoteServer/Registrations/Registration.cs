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
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    abstract class Registration
    {
        TCPAccepter m_accepter;
        protected Logger Logger { get; private set; }
        static protected FileUserBase DataBase { get; private set; } = new FileUserBase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\", "accounts.txt");
        static public SafeList<UserAccount> Accounts { get; private set; } = new SafeList<UserAccount>();
        public int AuthenticationTimeout { get; set; } = 30000;

        public Registration(TCPAccepter accepter)
        {
            m_accepter = accepter;

            Logger = new LoggerConsole();
            m_accepter.Logger = Logger;
            m_accepter.ClientConnected += OnClientConnected;
            m_accepter.Run();

            Logger.Log("Registration is now accepting connections on port " + m_accepter.Port);
        }

        void OnClientConnected(object sender, Socket acceptedSocket)
        {
            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);
            connection.Logger = Logger;
            connection.InitializeCrypto(new RSALegacyCrypto(false));
            if(!Authenticate(out UserAccount user, connection))
            {
                Logger.Log("Authentication failed.");
                connection.Disconnect();
                return;
            }

            HandleVerifiedConnection(user, connection); // DataBase.Instance.GetAccount("dummy")
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

                if (!DataBase.Validate(username, Encoding.ASCII.GetBytes(userSplit[1])))
                    return false;

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
            Accounts.Add(user);

            return user;
        }

        protected abstract void HandleVerifiedConnection(UserAccount user, IConnection clientConnection);
    }
}
