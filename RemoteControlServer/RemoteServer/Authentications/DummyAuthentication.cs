using System.Net.Sockets;
using RemoteServer.User;

namespace RemoteServer.Authentications
{
    class DummyAuthentication : Authentication
    {
        public DummyAuthentication()
        {
        }

        public override bool DoAuthentication(Socket socket, out UserAccount userAccount)
        {
            userAccount = DummyDataBase.Instance.GetAccount("");
            return true;
        }
    }
}
