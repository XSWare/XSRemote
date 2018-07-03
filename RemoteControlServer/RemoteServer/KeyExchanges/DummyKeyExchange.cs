using System.Net.Sockets;

namespace RemoteServer.KeyExchanges
{
    class DummyKeyExchange : KeyExchange
    {
        public override bool DoKeyExchange(Socket socket)
        {
            return true;
        }
    }
}
