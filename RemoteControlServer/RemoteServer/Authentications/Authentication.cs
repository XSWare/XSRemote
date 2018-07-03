using RemoteServer.User;
using System.Net.Sockets;

namespace RemoteServer.Authentications
{
    abstract class Authentication
    {
        public abstract bool DoAuthentication(Socket socket, out UserAccount userAccount);
    }
}
