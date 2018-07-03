using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using XSLibrary.Network.Connections;

namespace RemoteShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            bool reconnect = true;

            do
            {
                ConnectLoop(out Socket connection);

                DataReceiver dataReceiver = new DataReceiver(new TCPPacketConnection(connection));

                string command = "";
                while ((command = Console.In.ReadLine()) != "connect")
                {
                    if(command == "exit")
                    {
                        reconnect = false;
                        break;
                    }

                    dataReceiver.ManualCommand(command);
                }

                dataReceiver.Dispose();
            } while (reconnect);
        }

        static void ConnectLoop(out Socket conSocket)
        {
            while (!Connect(out conSocket))
            {
                Thread.Sleep(1000);
            }
        }

        static bool Connect(out Socket conSocket)
        {
            string ipAdress = "80.109.174.197";
            int port = 22222;
            if (!IPAddress.TryParse(ipAdress, out IPAddress IP))
            {
                Console.Out.WriteLine("Invalid IP format \"{0}\".", ipAdress);
                conSocket = null;
                return false;
            }

            conSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.Out.WriteLine("Connecting to {0}:{1}", ipAdress, port.ToString());
            try
            {
                conSocket.Connect(new IPEndPoint(IP, 22222));
                Console.Out.WriteLine("Connected.");
                return true;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to connect: {0}", e.Message);
                return false;
            }

        }
    }
}
