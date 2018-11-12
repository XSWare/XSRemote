using RemoteShutdown.CommandResolving;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using XSLibrary.Network.Connections;
using RemoteShutdownLibrary;
using XSLibrary.Utility;
using RemoteShutdown.Functionalty;

namespace RemoteShutdown
{
    class DataReceiver : IDisposable
    {
        public event EventHandler ServerDisconnect;

        TCPPacketConnection Connection { get; set; }
        CommandoExecutionActor m_commandExecutionActor;
        Thread m_keepAliveThread;

        Logger m_logger = Logger.NoLog;
        public Logger Logger
        {
            get { return m_logger; }
            set
            {
                m_logger = value;
                Connection.Logger = m_logger;
            }
        }

        public int KeepAliveInterval { get; set; } = 10000;
        int ShutdownCheckInterval { get; set; } = 100;

        public DataReceiver(TCPPacketConnection connection)
        {
            Connection = connection;
            Connection.DataReceivedEvent += OnConnectionReceive;
            Connection.OnDisconnect.Event += OnServerDisconnect;
        }

        public void Run()
        {
            List<CommandResolver> commandResolvers = new List<CommandResolver>()
            {
                new ShutdownCommandResolve(new ShutdownHandler()),
                new VolumeCommandResolver(new VolumeHandler()),
                new MediaPlayerResolver()
            };

            m_commandExecutionActor = new CommandoExecutionActor(commandResolvers);

            Connection.Logger = Logger;
            Connection.InitializeReceiving();

            m_keepAliveThread = new Thread(KeepAliveLoop);
            m_keepAliveThread.Name = "Keep alive";
            m_keepAliveThread.Start();
        }

        public void SendReply(string reply)
        {
            Connection.Send(TransmissionConverter.ConvertStringToByte(reply));
        }

        public void ManualCommand(string command)
        {
            m_commandExecutionActor.SendMessage(command);
        }

        private void OnServerDisconnect(object sender, EndPoint endpoint)
        {
            Logger.Log(LogLevel.Priority, "Disconnected from server.");
            ServerDisconnect?.Invoke(this, new EventArgs());
        }

        private void KeepAliveLoop()
        {
            int interval = 0;
            while (Connection.Connected)
            {
                Thread.Sleep(ShutdownCheckInterval);

                interval += ShutdownCheckInterval;
                if (interval >= KeepAliveInterval)
                {
                    interval = 0;
                    Connection.SendKeepAlive();
                }
            }
        }

        private void OnConnectionReceive(object sender, byte[] data, EndPoint source)
        {
            string command = GetCommandoFromBytes(data);
            Logger.Log(LogLevel.Information, "Received command \"{0}\"", command);
            SendReply("Received: " + command);
            m_commandExecutionActor.SendMessage(command);
        }

        private string GetCommandoFromBytes(byte[] data)
        {
            return TransmissionConverter.ConvertByteToString(data).Trim();
        }

        public void Dispose()
        {
            if(m_commandExecutionActor != null)
                m_commandExecutionActor.Stop(true);

            Connection.Disconnect();

            if (m_keepAliveThread != null && m_keepAliveThread.ThreadState != ThreadState.Unstarted)
                m_keepAliveThread.Join();
        }
    }
}
