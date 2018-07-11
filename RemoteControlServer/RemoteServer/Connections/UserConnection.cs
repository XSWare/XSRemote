using RemoteServer.User;
using RemoteShutdownLibrary;
using XSLibrary.Network.Connections;

namespace RemoteServer.Connections
{
    class UserConnection : ConnectionBase
    {
        UserAccount m_userAccount;

        public UserConnection(ConnectionInterface connection, UserAccount userAccount) : base(connection)
        {
            m_userAccount = userAccount;
        }

        public override void Send(string data)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(data));
            Logger.Log("Send \"{0}\" to user: {1}", data, m_userAccount.UserData.Username);
        }

        protected override void ReceiveCommand(string command)
        {
            Logger.Log("Received from user: {0}", command);

            if (WantsDeviceList(command))
            {
                string deviceList = m_userAccount.GetDeviceList();
                Send(deviceList);
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
