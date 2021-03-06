﻿using RemoteServer.Accounts;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Network.Connections;

namespace RemoteServer.Registrations
{
    class AdminRegistration : Registration<AdminAccount>
    {
        public AdminRegistration(TCPAcceptor accepter, AdminPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts, dataBase)
        {
        }

        protected override void HandleVerifiedConnection(AdminAccount user, TCPPacketConnection clientConnection)
        {
            if (user.SetConnection(clientConnection))
            {
                clientConnection.DataReceivedEvent += user.OnCommandReceived;
                clientConnection.InitializeReceiving();
                clientConnection.OnDisconnect.Event += user.OnAdminDisconnect;
            }
            else
                clientConnection.Disconnect();
        }
    }
}
