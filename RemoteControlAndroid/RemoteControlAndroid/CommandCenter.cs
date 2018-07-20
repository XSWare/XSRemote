﻿using RemoteShutdowLibrary;
using RemoteShutdownLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;

namespace RemoteControlAndroid
{
    class CommandCenter
    {
        public static event OnDisconnectEvent.EventHandle OnDisconnect;
        public static event IConnection.DataReceivedHandler OnDataReceived;

        public static CommandCenter Instance { get; private set; } = new CommandCenter();

        public static bool Connected { get { return Instance.m_connection != null && Instance.m_connection.Connected; } }
        public string LastError { get; private set; } = "";
        public int KeepAliveInterval { get; private set; } = 10000;

        TCPPacketConnection m_connection = null;
        EndPoint m_lastLogin = null;

        private CommandCenter()
        {
        }

        public void Connect(EndPoint remote, Action callback)
        {
            m_lastLogin = null;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try { socket.Connect(remote); }
            catch
            {
                LastError = "Failed to connect.";
                callback();
                return;
            }

            if (m_connection != null)
                m_connection.OnDisconnect -= HandleDisconnect;

            m_connection = new TCPPacketConnection(socket);
            if (!m_connection.InitializeCrypto(new RSALegacyCrypto(true), 30000))
            {
                LastError = "Handshake failed.";
                callback();
                return;
            }

            m_connection.SendTimeout = 5000;
            m_connection.Send(Encoding.ASCII.GetBytes("dave Gratuliere123!"));
            if(!m_connection.Receive(out byte[] data, out EndPoint source, 5000) || data[0] != '+')
            {
                LastError = "Authentication failed.";
                m_connection.Disconnect();
                callback();
                return;
            }

            m_lastLogin = remote;

            m_connection.OnDisconnect += HandleDisconnect;
            m_connection.DataReceivedEvent += HandleDataReceived;
            m_connection.InitializeReceiving();
            callback();
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
            if (Connected)
            {
                Instance.m_lastLogin = null;
                Instance.m_connection.Disconnect();
            }
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

        private bool Reconnect()
        {
            if (m_lastLogin == null)
                return false;

            Connect(m_lastLogin, () => { });
            return Connected;
        }
    }
}