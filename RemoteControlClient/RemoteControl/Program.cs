using System;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new LoggerConsole();


            LoopingConnector connector = new LoopingConnector();
            connector.Logger = logger;

            bool reconnect = true;

            do
            {
#if DEBUG
                logger.LogLevel = LogLevel.Detail;
#else
                logger.LogLevel = LogLevel.Error;
#endif
                if (!connector.ConnectLoop(out TCPPacketConnection connection))
                    break;

                logger.Log(LogLevel.Priority, "Connected to server.");

                DataReceiver dataReceiver = new DataReceiver(connection);
                dataReceiver.Logger = logger;
                dataReceiver.Run();

                string command = "";
                while ((command = Console.In.ReadLine()) != "connect")
                {
                    if(command == "exit")
                    {
                        reconnect = false;
                        break;
                    }

                    if (command == "reconnect")
                        break;

                    if (command.StartsWith("send ") && command.Length > 5)
                    {
                        logger.Log(LogLevel.Priority, "Sending to server: {0}", command.Substring(5));
                        dataReceiver.SendReply(command.Substring(5));
                    }
                    else
                    {
                        logger.Log(LogLevel.Priority, "Executing on device: {0}", command);
                        dataReceiver.ManualCommand(command);
                    }
                }

                dataReceiver.Dispose();
            } while (reconnect);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
