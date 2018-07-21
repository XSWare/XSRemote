using System;
using System.Net;
using System.Threading;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;
using XSLibrary.Network.Connectors;
using XSLibrary.Utility;

namespace RemoteShutdown
{
    class LoopingConnector
    {
        AccountConnector m_connector;

        Logger m_logger = Logger.NoLog;
        public Logger Logger
        {
            get { return m_logger; }
            set
            {
                m_logger = value;
                m_connector.Logger = m_logger;
            }
        }

        public LoopingConnector()
        {
            m_connector = CreateConnector();
        }

        public bool ConnectLoop(out TCPPacketConnection connection, int retries = 10)
        {
            connection = null;

            int tryCount = 0;
            int retryPause = 1000;

            while (tryCount < retries)
            {
                if (Connect(out connection))
                    break;

                Logger.Log(LogLevel.Error, "Connect try {0} out of {1} failed.", tryCount + 1, retries);
                Thread.Sleep(retryPause);
                tryCount++;
            }

            return connection != null && connection.Connected;
        }

        private AccountConnector CreateConnector()
        {
            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.RSALegacy;
            connector.Login = "dave Gratuliere123!";

            return connector;
        }

        public bool Connect(out TCPPacketConnection connection)
        {
            connection = null;

            if (m_connector.Login.Length <= 0)
            {
                Console.WriteLine("Enter login:");
                m_connector.Login = Console.ReadLine();
            }

            string ipAdress = "80.109.174.197";
            int port = 80;
            if (!IPAddress.TryParse(ipAdress, out IPAddress IP))
            {
                Logger.Log(LogLevel.Error, "Invalid IP format \"{0}\".", ipAdress);
                return false;
            }

            return m_connector.Connect(new IPEndPoint(IP, port), out connection);
        }
    }
}
