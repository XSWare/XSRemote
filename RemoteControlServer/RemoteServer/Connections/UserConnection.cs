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

            if (WantsDeviceList(command))
            {
                string deviceList = m_userAccount.GetDeviceList();
                SendCommand(deviceList);
            }
            else
                m_userAccount.BroadCastCommand(command);
        }

        bool WantsDeviceList(string command)
        {
            return command == "getdevicelist";
        }
    }
}
