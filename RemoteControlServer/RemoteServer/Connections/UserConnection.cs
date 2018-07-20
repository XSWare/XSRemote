using RemoteServer.User;
using RemoteShutdownLibrary;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Connections
{
    class UserConnection : ConnectionBase
    {
        UserAccount m_userAccount;

        public UserConnection(IConnection connection, UserAccount userAccount) : base(connection)
        {
            m_userAccount = userAccount;
            OnDataReceived += ReceiveCommand;
        }

        public override void Send(string data)
        {
            m_connection.Send(TransmissionConverter.ConvertStringToByte(data));
            Logger.Log(LogLevel.Information, "Send \"{0}\" to user: {1}", data, m_userAccount.Username);
        }

        void ReceiveCommand(object sender, string data)
        {
            Logger.Log(LogLevel.Information, "Received from user \"{0}\": {1}", m_userAccount.Username, data);

            if (WantsDeviceList(data))
            {
                string deviceList = m_userAccount.GetDeviceList();
                Send(deviceList);
            }
            else
                m_userAccount.BroadCastCommand(data);
        }

        bool WantsDeviceList(string command)
        {
            return command == "getdevicelist";
        }
    }
}
