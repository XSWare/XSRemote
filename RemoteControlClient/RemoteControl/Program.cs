using System;
using XSLibrary.Network.Connections;

namespace RemoteShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            Connector connector = new Connector();

            bool reconnect = true;

            do
            {
                if (!connector.ConnectLoop(out TCPPacketConnection connection))
                    break;

                DataReceiver dataReceiver = new DataReceiver(connection);
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
                        dataReceiver.SendReply(command.Substring(5));
                    else
                        dataReceiver.ManualCommand(command);
                }

                dataReceiver.Dispose();
            } while (reconnect);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
