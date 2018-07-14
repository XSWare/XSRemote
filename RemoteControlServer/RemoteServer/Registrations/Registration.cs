using RemoteServer.User;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Registrations
{
    abstract class Registration
    {
        TCPAccepter m_accepter;
        protected Logger Logger { get; private set; }
        static protected FileUserBase DataBase { get; private set; } = new FileUserBase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\", "acounts.txt");

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
            if(!Authenticate(out UserData user, connection))
            {
                connection.Disconnect();
                return;
            }

            HandleVerifiedConnection(new UserAccount(user), connection); // DataBase.Instance.GetAccount("dummy")
        }

        bool Authenticate(out UserData user, IConnection connection)
        {
            user = null;

            try
            {
                if (!connection.Receive(out byte[] userData, out EndPoint source))
                    return false;

                string userString = Encoding.ASCII.GetString(userData);
                string[] userSplit = userString.Split(' ');

                if (userSplit.Length != 2)
                    return false;

                return DataBase.Validate(userSplit[0], HexStringConverter.ToBytes(userSplit[1]));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected abstract void HandleVerifiedConnection(UserAccount user, IConnection clientConnection);
    }
}
