using System.Net.Sockets;

namespace RemoteServer.KeyExchanges
{
    abstract class KeyExchange
    {
        public abstract bool DoKeyExchange(Socket socket);
    }
}
