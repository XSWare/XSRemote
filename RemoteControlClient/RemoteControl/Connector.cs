using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;

namespace RemoteShutdown
{
    class RepeatConnector
    {
        AccountConnector m_connector;

        public bool ConnectLoop(out TCPPacketConnection connection, int retries = 10)
        {
            connection = null;

            m_connector = CreateConnector();

            int tryCount = 0;
            int retryPause = 1000;

            while (tryCount < retries)
            {
                if (Connect(out connection, out string message))
                {
                    Console.WriteLine(message);
                    break;
                }

                if (message == AccountConnector.AUTHENTICATION_FAILED)
                    m_connector.Login = "";

                Console.WriteLine("Connect try {0} out of {1} failed: {2}", tryCount + 1, retries, message);
                Thread.Sleep(retryPause);
                tryCount++;
            }

            return connection != null && connection.Connected;
        }

        private AccountConnector CreateConnector()
        {
            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.RSALegacy;

            return connector;
        }

        public bool Connect(out TCPPacketConnection connection, out string message)
        {
            connection = null;
            message = "";

            if (m_connector.Login.Length <= 0)
            {
                Console.WriteLine("Enter login:");
                m_connector.Login = Console.ReadLine();
            }

            string ipAdress = "80.109.174.197";
            int port = 80;
            if (!IPAddress.TryParse(ipAdress, out IPAddress IP))
            {
                Console.Out.WriteLine("Invalid IP format \"{0}\".", ipAdress);
                return false;
            }

            Console.Out.WriteLine("Connecting to {0}:{1}", ipAdress, port.ToString());

            return m_connector.Connect(new IPEndPoint(IP, port), out connection, out message);
        }
    }
}
