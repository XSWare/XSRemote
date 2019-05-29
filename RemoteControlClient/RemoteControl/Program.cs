using RemoteShutdown.CommandResolving;
using RemoteShutdown.Functionalty;
using System;
using System.Collections.Generic;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteShutdown
{
    class Program
    {
        static Logger m_logger = new LoggerConsole();
        static CommandoExecutionActor m_commandoExecutionActor;

        static void Main(string[] args)
        {
#if DEBUG
            m_logger.LogLevel = LogLevel.Detail;
#else
            m_logger.LogLevel = LogLevel.Error;
#endif

            List<CommandResolver> commandResolvers = new List<CommandResolver>()
            {
                new ShutdownCommandResolve(new ShutdownHandler()),
                new VolumeCommandResolver(new VolumeHandler()),
                new MediaPlayerResolver()
            };
            m_commandoExecutionActor = new CommandoExecutionActor(commandResolvers);

            bool reconnect = true;

            do
            {
                DataReceiver dataReceiver = Connect();
                if (dataReceiver != null)
                {
                    reconnect = OnlineCommandLoop(dataReceiver);
                    dataReceiver.Dispose();
                }
                else
                    reconnect = LocalCommandLoop();
                
            } while (reconnect);

            m_commandoExecutionActor.Stop(true);
        }

        static DataReceiver Connect()
        {
            LoopingConnector connector = new LoopingConnector();
            connector.Logger = m_logger;

            if (!connector.ConnectLoop(out TCPPacketConnection connection))
                return null;

            m_logger.Log(LogLevel.Priority, "Connected to server.");

            DataReceiver dataReceiver = new DataReceiver(connection, m_commandoExecutionActor);
            dataReceiver.Logger = m_logger;
            dataReceiver.Run();

            return dataReceiver;
        }

        static bool OnlineCommandLoop(DataReceiver dataReceiver)
        {
            string command = "";
            while ((command = Console.In.ReadLine()) != "connect")
            {
                if (command == "exit")
                    return false;

                if (command == "reconnect")
                    return true;

                if (command.StartsWith("send ") && command.Length > 5)
                {
                    m_logger.Log(LogLevel.Priority, "Sending to server: {0}", command.Substring(5));
                    dataReceiver.SendReply(command.Substring(5));
                }
                else
                {
                    m_logger.Log(LogLevel.Priority, "Executing on device: {0}", command);
                    dataReceiver.ManualCommand(command);
                }
            }

            return true;
        }

        static bool LocalCommandLoop()
        {
            Console.WriteLine("Unable to connect. Continuing in local mode.");

            string command = "";
            while ((command = Console.In.ReadLine()) != "connect")
            {
                if (command == "exit")
                    return false;

                m_logger.Log(LogLevel.Priority, "Executing on device: {0}", command);
                m_commandoExecutionActor.SendMessage(command);

            }

            return true;
        }
    }
}
