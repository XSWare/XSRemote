﻿using RemoteShutdownLibrary;
using System.Net;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    abstract class ConnectionBase
    {
        public delegate void ClientDisconnectHandler(object sender);
        public event ClientDisconnectHandler OnDisconnect;

        protected IConnection m_connection;

        public Logger Logger { get; set; }

        public ConnectionBase(IConnection connection)
        {
            Logger = new LoggerConsole();

            m_connection = connection;

            m_connection.DataReceivedEvent += ReceiveData;
            m_connection.OnDisconnect += OnConnectionLoss;
        }

        public bool Initialize()
        {
            if (!m_connection.InitializeCrypto(new RSALegacyCrypto(false)))
                return false;

            m_connection.InitializeReceiving();
            return true;
        }

        public virtual void Send(string command)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(command));
        }

        protected abstract void ReceiveCommand(string command);

        private void ReceiveData(object sender, byte[] data, EndPoint source)
        {
            ReceiveCommand(TransmissionConverter.ConvertByteToString(data));
        }

        protected void OnConnectionLoss(object sender, EndPoint endPoint)
        {
            OnDisconnect?.Invoke(this);
        }

    }
}
