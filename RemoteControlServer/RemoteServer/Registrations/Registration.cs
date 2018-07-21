using RemoteServer.User;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Registrations;

namespace RemoteServer.Registrations
{
    abstract class Registration : IRegistration<TCPPacketConnection, UserAccount>
    {
        delegate void DisposeHandler();
        event DisposeHandler OnDispose;

        public Registration(TCPAccepter accepter, IUserDataBase dataBase, AccountPool accounts)
            : base(accepter, dataBase, accounts)
        {
            Crypto = CryptoType.RSALegacy;
            AuthenticationTimeout = 30000;
        }

        protected override TCPPacketConnection CreateConnection(Socket acceptedSocket)
        {
            TCPPacketConnection connection = new TCPPacketConnection(acceptedSocket);
            OnDispose += connection.Dispose;
            connection.OnDisconnect += DisconnectHandler;

            return connection;
        }

        protected override bool Authenticate(out UserAccount user, TCPPacketConnection connection)
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

                connection.Send(new byte[1] { (byte)'+' }, 30000);

                user = Accounts.GetAccount(username);
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
