using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;

namespace RemoteShutdown
{
    class Connector
    {
        string login = "";

        public bool ConnectLoop(out TCPPacketConnection connection, int retries = 10)
        {
            connection = null;

            int tryCount = 0;
            int retryPause = 1000;

            while (tryCount < retries && !Connect(out connection))
            {
                Console.WriteLine("Connect try {0} out of {1} failed.", tryCount + 1, retries);
                Thread.Sleep(retryPause);
                tryCount++;
            }

            return connection != null && connection.Connected;
        }

        public bool Connect(out TCPPacketConnection connection)
        {
            connection = null;

            if (login.Length <= 0)
            {
                Console.WriteLine("Enter login:");
                login = Console.ReadLine();
            }

            string ipAdress = "80.109.174.197";
            int port = 80;
            if (!IPAddress.TryParse(ipAdress, out IPAddress IP))
            {
                Console.Out.WriteLine("Invalid IP format \"{0}\".", ipAdress);
                return false;
            }

            Socket conSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.Out.WriteLine("Connecting to {0}:{1}", ipAdress, port.ToString());
            try
            {
                conSocket.Connect(new IPEndPoint(IP, port));
                connection = new TCPPacketConnection(conSocket);
                Handshake(connection);
                Console.Out.WriteLine("Connected.");
                return true;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to connect: {0}", e.Message);
                return false;
            }

        }

        void Handshake(TCPPacketConnection connection)
        {
            if (!connection.InitializeCrypto(new RSALegacyCrypto(true)))
                throw new ConnectionException("Crypto init failed!");

            connection.Send(Encoding.ASCII.GetBytes(login));
            if (!connection.Receive(out byte[] data, out EndPoint source) || data[0] != '+')
            {
                login = "";
                throw new ConnectionException("Authentication failed!");
            }
        }
    }
}
