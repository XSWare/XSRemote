﻿using RemoteShutdown.CommandResolving;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using XSLibrary.Network.Connections;
using RemoteShutdownLibrary;
using XSLibrary.Network.ConnectionCryptos;

namespace RemoteShutdown
{
    class DataReceiver : IDisposable
    {
        public event EventHandler ServerDisconnect;

        TCPPacketConnection m_serverConnection;
        CommandoExecutionActor m_commandExecutionActor;

        CryptoModule m_crypto;

        public DataReceiver(TCPPacketConnection serverConnection)
        {
            m_serverConnection = serverConnection;
            if (!m_serverConnection.InitializeCrypto(new ECCrypto(true)))
                throw new Exception("Crypto init failed!");

            List<CommandResolver> commandResolvers = new List<CommandResolver>()
            {
                new ShutdownCommandResolve(new ShutdownHandler()),
                new VolumeCommandResolver(new VolumeHandler()),
                new MediaPlayerResolver()
            };

            m_commandExecutionActor = new CommandoExecutionActor(commandResolvers);

            m_crypto = new CryptoModule();

            m_serverConnection.DataReceivedEvent += OnConnectionReceive;
            m_serverConnection.OnReceiveError += OnServerDisconnect;
            m_serverConnection.InitializeReceiving();

            Thread keepAliveThread = new Thread(KeepAliveLoop);
            keepAliveThread.Name = "Keep alive";
            keepAliveThread.Start();
        }

        public void SendReply(string reply)
        {
            m_serverConnection.Send(m_crypto.Encrypt(TransmissionConverter.ConvertStringToByte(reply)));
        }

        public void ManualCommand(string command)
        {
            m_commandExecutionActor.SendMessage(command);
        }

        private void OnServerDisconnect(object sender, IPEndPoint endpoint)
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

        private void OnConnectionReceive(object sender, byte[] data, IPEndPoint source)
        {
            string command = GetCommandoFromBytes(data);
            Console.Out.WriteLine("Received command \"{0}\"", command);
            SendReply("Received: " + command);
            m_commandExecutionActor.SendMessage(command);
        }

        private string GetCommandoFromBytes(byte[] data)
        {
            byte[] decryptedData = m_crypto.Decrypt(data);
            return TransmissionConverter.ConvertByteToString(decryptedData).Trim();
        }

        public void Dispose()
        {
            m_commandExecutionActor.Stop();
            m_serverConnection.Disconnect();
        }
    }
}
