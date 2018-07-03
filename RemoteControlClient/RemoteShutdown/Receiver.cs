using RemoteShutdown.CommandResolving;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using XSLibrary.Network.Connections;
using RemoteShutdownLibrary;

namespace RemoteShutdown
{
    class DataReceiver : IDisposable
    {
        public event EventHandler ServerDisconnect;

        TCPConnection m_serverConnection;
        CommandoExecutionActor m_commandExecutionActor;

        CryptoModule m_crypto;

        public DataReceiver(TCPConnection serverConnection)
        {
            m_serverConnection = serverConnection;

            List<CommandResolver> commandResolvers = new List<CommandResolver>()
            {
                new ShutdownCommandResolve(new ShutdownHandler()),
                new VolumeCommandResolver(new VolumeHandler()),
                new MediaPlayerResolver()
            };

            m_commandExecutionActor = new CommandoExecutionActor(commandResolvers);

            m_crypto = new CryptoModule();

            m_serverConnection.DataReceivedEvent += OnConnectionReceive;
            m_serverConnection.ReceiveErrorEvent += OnServerDisconnect;
            m_serverConnection.InitializeReceiving();

            new Thread(KeepAliveLoop).Start();
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

        private void OnConnectionReceive(object sender, byte[] data)
        {
            string command = GetCommandoFromBytes(data);
            Console.Out.WriteLine("Received command \"{0}\"", command);
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
