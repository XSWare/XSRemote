﻿using RemoteShutdowLibrary;
using RemoteShutdownLibrary;
using System;
using System.Net;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.Utility;

namespace RemoteControlAndroid
{
    class CommandCenter
    {
        public delegate void ConnectHandle();

        public static event ConnectHandle OnConnect;
        public static event OnDisconnectEvent.EventHandle OnDisconnect;
        public static event IConnection.DataReceivedHandler OnDataReceived;

        public static CommandCenter Instance { get; private set; } = new CommandCenter();

        public static bool Connected { get { return Instance.m_connection != null && Instance.m_connection.Connected; } }
        public static bool CurrentlyConnecting { get { return Instance.m_connector.CurrentlyConnecting; } }
        public static bool DisconnectedGracefully { get { return Instance.m_disconnectedGracefully; } }
        public int KeepAliveInterval { get; private set; } = 10000;

        Logger m_logger = Logger.NoLog;
        public static Logger ActiveLogger
        {
            get { return Instance.m_logger; }
            set
            {
                Instance.m_logger = value;
                Instance.m_connector.Logger = value;
            }
        }

        AccountConnector m_connector;
        TCPPacketConnection m_connection = null;

        bool m_disconnectedGracefully = false;

        private CommandCenter()
        {
            m_connector = CreateConnector();
        }

        private AccountConnector CreateConnector()
        {
            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.RSALegacy;
            connector.TimeoutCryptoHandshake = 30000;
            connector.Login = "dave Gratuliere123!";

            return connector;
        }

        public static void Connect(EndPoint remote)
        {
            Instance.m_disconnectedGracefully = false;

            Action<TCPPacketConnection> successCallback = ((connection) =>
            {
                Instance.SetConnection(connection);
                OnConnect?.Invoke();
            });

            Instance.m_connector.ConnectAsync(remote, successCallback, () => { });
        }

        void SetConnection(TCPPacketConnection connection)
        {
            Instance.m_connection = connection;
            connection.DataReceivedEvent += Instance.HandleDataReceived;
            connection.InitializeReceiving();
            connection.OnDisconnect += Instance.HandleDisconnect;
        }

        public static void SendControlCommand(string cmd)
        {
            Instance.SendCommand(Commands.CONTROL + " " + cmd);
        }

        public static void SendMediaCommand(string cmd)
        {
            Instance.SendCommand(Commands.MEDIA + " " + cmd);
        }

        public static void SendVolumeCommand(string cmd)
        {
            Instance.SendCommand(Commands.VOLUME + " " + cmd);
        }

        private void SendCommand(string cmd)
        {
            if (!Connected)
                return;

            m_connection.Send(TransmissionConverter.ConvertStringToByte(cmd));
        }

        public static void Disconnect()
        {
            Instance.m_disconnectedGracefully = true;

            if (Connected)
                Instance.m_connection.Disconnect();
        }

        public static void SendKeepAlive()
        {
            if (Connected)
                Instance.m_connection.SendKeepAlive();
        }

        private void HandleDataReceived(object sender, byte[] data, EndPoint source)
        {
            OnDataReceived?.Invoke(this, data, source);
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            //if(!Reconnect())
                OnDisconnect?.Invoke(this, remote);
        }

        //private bool Reconnect()
        //{
        //    if (m_lastLogin == null)
        //        return false;

        //    Connect(m_lastLogin, () => { });
        //    return Connected;
        //}
    }
}