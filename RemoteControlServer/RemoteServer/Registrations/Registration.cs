using RemoteServer.User;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Acceptors;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;

namespace RemoteServer.Registrations
{
    abstract class Registration : IRegistration<TCPPacketConnection, UserAccount>
    {
        delegate void DisposeHandler();
        event DisposeHandler OnDispose;

        IUserDataBase DataBase { get; set; }

        public Registration(TCPAcceptor accepter, UserPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts)
        {
            DataBase = dataBase;
            Crypto = CryptoType.RSALegacy;
            CryptoHandshakeTimeout = 30000;
        }

        protected override TCPPacketConnection CreateConnection(Socket acceptedSocket)
        {
            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);
            OnDispose += connection.Dispose;
            connection.OnDisconnect += DisconnectHandler;

            return connection;
        }

        protected override bool Authenticate(out string username, TCPPacketConnection connection)
        {
            username = "";

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

                username = userSplit[0];

                if (!DataBase.Validate(username, Encoding.ASCII.GetBytes(userSplit[1]), Accounts.AccessLevel))
                    return false;

                connection.Send(new byte[1] { (byte)'+' }, 30000);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void DisconnectHandler(object sender, EndPoint remote)
        {
            IConnection connection = sender as IConnection;
            if (connection != null)
                OnDispose -= connection.Dispose;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnDispose?.Invoke();
        }
    }
}
