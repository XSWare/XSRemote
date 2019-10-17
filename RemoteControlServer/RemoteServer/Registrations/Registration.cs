using System;
using System.Net;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Acceptors;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;

namespace RemoteServer.Registrations
{
    abstract class Registration<AccountType> : IRegistration<AccountType> where AccountType : IUserAccount
    {
        public int KeepAliveTime { get; set; } = 10000;
        public int KeepAliveInterval { get; set; } = 5000;

        IUserDataBase DataBase { get; set; }

        public Registration(TCPAcceptor accepter, IAccountPool<AccountType> accounts, IUserDataBase dataBase)
            : base(new SecureAcceptor(accepter), accounts)
        {
            DataBase = dataBase;
            Crypto = CryptoType.EC25519;
            CryptoHandshakeTimeout = 30000;
        }

        protected override bool Authenticate(out string username, TCPPacketConnection connection)
        {
            username = "";

            try
            {
                if (!connection.Receive(out byte[] userData, out EndPoint source, AuthenticationTimeout))
                    return false;

                string userString = Encoding.ASCII.GetString(userData);
                string[] userSplit = userString.Split(' ');

                if (userSplit.Length != 2)
                    return false;

                username = userSplit[0];

                if (!DataBase.Validate(username, Encoding.ASCII.GetBytes(userSplit[1]), Accounts.AccessLevel))
                    return false;

                connection.Send(new byte[1] { (byte)'+' }, AuthenticationTimeout);

                connection.SetUpKeepAlive(KeepAliveTime, KeepAliveInterval);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
