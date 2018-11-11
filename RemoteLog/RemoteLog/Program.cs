using RemoteShutdownLibrary;
using System;
using System.Net;
using System.Text;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.Utility;

namespace RemoteLog
{
    class Program
    {
        static LoggerConsolePeriodic logger = new LoggerConsolePeriodic("[LOCAL] ", "\n");
        static LoggerConsolePeriodic connectionLogger = new LoggerConsolePeriodic("[LOCAL] ", "\n");
        static LoggerConsole networkLog = new LoggerConsole();
        static SafeList<IConnection> connections = new SafeList<IConnection>();

        static void Main(string[] args)
        {
            logger.LogLevel = LogLevel.Information;
            connectionLogger.LogLevel = LogLevel.Error;

            Connect("127.0.0.1", 22224, "admin secure");

            string command = "";
            while ((command = Console.ReadLine()) != "exit")
            {
                if (connections.Count > 0)
                {
                    foreach(IConnection connection in connections.Entries)
                    {
                        connection.Send(TransmissionConverter.ConvertStringToByte(command));
                    }
                }
                else
                {
                    string[] split = command.Split(' ');
                    if (split.Length != 4 || !Int32.TryParse(split[1], out int port))
                    {
                        logger.Log(LogLevel.Priority, "Invalid command format.");
                        continue;
                    }

                    string login = split[2] + " " + split[3];

                    Connect(split[0], port, login);
                }
            }

            foreach(IConnection connection in connections.Entries)
            {
                connection.Disconnect();
            }
        }

        private static void Connect(string ip, int port, string login)
        {
            EndPoint remote;
            try { remote = new IPEndPoint(IPAddress.Parse(ip), port); }
            catch
            {
                logger.Log(LogLevel.Priority, "Invalid IP format.");
                return;
            }

            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.RSALegacy;
            connector.Logger = logger;
            connector.Login = login;

            if (!connector.Connect(remote, out TCPPacketConnection connection))
                return;

            connections.Add(connection);
            connection.Logger = connectionLogger;
            connection.DataReceivedEvent += Connection_DataReceivedEvent;
            connection.InitializeReceiving();
            connection.OnDisconnect.Event += HandleDisconnect;
        }

        private static void HandleDisconnect(object sender, EndPoint arguments)
        {
            connections.Remove(sender as IConnection);
            networkLog.Log(LogLevel.Priority, "[{0}] Connection closed by remote.", arguments);
        }

        private static void Connection_DataReceivedEvent(object sender, byte[] data, EndPoint source)
        {
            networkLog.Log(LogLevel.Priority, "[{0}] {1}", source, Encoding.ASCII.GetString(data));
        }
    }
}
