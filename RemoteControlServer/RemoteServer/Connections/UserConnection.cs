using RemoteServer.User;
using XSLibrary.Network.Connections;

namespace RemoteServer.Connections
{
    class UserConnection : ConnectionBase
    {
        UserAccount m_userAccount;

        public UserConnection(TCPPacketConnection connection, UserAccount userAccount) : base(connection)
        {
            m_userAccount = userAccount;
        }

        public override void SendCommand(string command)
        {
            m_userAccount.BroadCastCommand(command);
        }

        protected override void ReceiveCommand(string command)
        {
            Logger.Log("Received from user: {0}", command);

            m_userAccount.BroadCastCommand(command);
        }
    }
}
