﻿using RemoteShutdown.CommandResolving;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using XSLibrary.Network.Connections;
using RemoteShutdownLibrary;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Utility;
using System.Text;

namespace RemoteShutdown
{
    class DataReceiver : IDisposable
    {
        public event EventHandler ServerDisconnect;

        TCPPacketConnection m_serverConnection;
        CommandoExecutionActor m_commandExecutionActor;

        public DataReceiver(TCPPacketConnection serverConnection)
        {
            m_serverConnection = serverConnection;
#if DEBUG
            m_serverConnection.Logger = new LoggerConsole();
#endif
            m_serverConnection.DataReceivedEvent += OnConnectionReceive;
            m_serverConnection.OnReceiveError += OnServerDisconnect;

            if (!m_serverConnection.InitializeCrypto(new RSALegacyCrypto(true)))
                throw new Exception("Crypto init failed!");

            m_serverConnection.Send(Encoding.ASCII.GetBytes("dave Gratuliere123!"));
            if (!m_serverConnection.Receive(out byte[] data, out EndPoint source) || data[0] != '+')
                throw new Exception("Authentication failed!");

            List<CommandResolver> commandResolvers = new List<CommandResolver>()
            {
                new ShutdownCommandResolve(new ShutdownHandler()),
                new VolumeCommandResolver(new VolumeHandler()),
                new MediaPlayerResolver()
            };

            m_commandExecutionActor = new CommandoExecutionActor(commandResolvers);

            m_serverConnection.InitializeReceiving();

            Thread keepAliveThread = new Thread(KeepAliveLoop);
            keepAliveThread.Name = "Keep alive";
            keepAliveThread.Start();
        }

        public void SendReply(string reply)
        {
            m_serverConnection.Send(TransmissionConverter.ConvertStringToByte(reply));
        }

        public void ManualCommand(string command)
        {
            m_commandExecutionActor.SendMessage(command);
        }

        private void OnServerDisconnect(object sender, EndPoint endpoint)
        {
            TCPConnection connection = sender as TCPConnection;
            connection.Disconnect();
            Console.Out.WriteLine("Disconnected from server.");
            ServerDisconnect?.Invoke(this, new EventArgs());
        }

        private void KeepAliveLoop()
        {
            while (m_serverConnection.Connected)
            {
                Thread.Sleep(10000);
                m_serverConnection.SendKeepAlive();
            }
        }

        private void OnConnectionReceive(object sender, byte[] data, EndPoint source)
        {
            string command = GetCommandoFromBytes(data);
            Console.Out.WriteLine("Received command \"{0}\"", command);
            SendReply("Received: " + command);
            m_commandExecutionActor.SendMessage(command);
        }

        private string GetCommandoFromBytes(byte[] data)
        {
            return TransmissionConverter.ConvertByteToString(data).Trim();
        }

        public void Dispose()
        {
            m_commandExecutionActor.Stop();
            m_serverConnection.Disconnect();
        }
    }
}
