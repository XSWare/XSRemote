using RemoteServer.Connections;
using RemoteServer.Device;
using System.Net;
using XSLibrary.Network.Registrations;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.Utility;

namespace RemoteServer.User
{
    class UserAccount: IUserAccount
    {
        public override event MemoryReleaseHandler OnMemoryCleanUp;

        SafeList<ControllableDevice> m_devices;
        UserConnection m_userConnection;

        public bool UserConnected { get { return m_userConnection != null; } }

        public UserAccount(string username) : base(username)
        {
            m_devices = new SafeList<ControllableDevice>();
        }

        public void SetUserConnection(UserConnection connection)
        {
            Logger.Log(LogLevel.Priority, "User connected to account \"{0}\".", Username);

            RemoveUserConnection();
            m_userConnection = connection;
            connection.OnDisconnect += HandleUserDisconnect;
        }

        private void HandleUserDisconnect(object sender, EndPoint remote)
        {
            Logger.Log(LogLevel.Priority, "User disconnected from account \"{0}\".", Username);
            RemoveUserConnection();
        }

        private void RemoveUserConnection()
        {
            if (m_userConnection == null)
                return;

            m_userConnection.OnDisconnect -= HandleUserDisconnect;
            m_userConnection = null;

            OnMemoryCleanUp?.Invoke(this);
        }

        public void AddDevice(ControllableDevice device)
        {
            device.OnDeviceDisconnect += DeviceDisconnecting;
            device.OnCommandReceived += HandleDeviceReply;
            m_devices.Add(device);
            Logger.Log(LogLevel.Priority, "Added device {0} to user \"{1}\".", device.DeviceID, Username);
        }

        public void RemoveDevice(ControllableDevice device)
        {
            m_devices.Remove(device);
            device.OnCommandReceived -= HandleDeviceReply;
            device.OnDeviceDisconnect -= DeviceDisconnecting;
            OnMemoryCleanUp?.Invoke(this);
        }

        public override bool StillInUse()
        {
            return m_devices.Count > 0 || m_userConnection != null;
        }

        public void SendCommand(int deviceID, string command)
        {
            foreach (ControllableDevice device in m_devices.Entries)
            {
                if(device.DeviceID == deviceID)
                {
                    device.SendCommand(command);
                    break;
                }
            }
        }

        public void BroadCastCommand(string command)
        {
            foreach (ControllableDevice device in m_devices.Entries)
                device.SendCommand(command);
        }

        public string GetDeviceList()
        {
            string deviceListReply = "devicelist";

            foreach (ControllableDevice device in m_devices.Entries)
            {
                deviceListReply += " " + device.DeviceID;
            }

            return deviceListReply;
        }

        private void HandleDeviceReply(object sender, string reply)
        {
            ControllableDevice device = sender as ControllableDevice;

            Logger.Log(LogLevel.Information, "Received reply \"{0}\" from device {1} for user \"{2}\".", reply, device.DeviceID, Username);

            if (UserConnected)
                m_userConnection.Send(string.Format("Device {0} - {1}", device.DeviceID, reply));
        }

        private void DeviceDisconnecting(object sender, EndPoint remote)
        {
            ControllableDevice device = sender as ControllableDevice;
            Logger.Log(LogLevel.Priority, "Device {0} disconnected from user \"{1}\".", device.DeviceID, Username);
            RemoveDevice(device);
        }
    }
}
