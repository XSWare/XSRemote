using RemoteServer.Connections;
using RemoteServer.Device;
using System;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.Utility;

namespace RemoteServer.User
{
    class UserAccount
    {
        public Logger Log { get; set; }
        public UserData UserData { get; private set; }
        SafeList<ControllableDevice> m_devices;
        UserConnection m_userConnection;

        public bool UserConnected { get { return m_userConnection != null; } }

        public UserAccount(UserData userData)
        {
            UserData = userData;

            Log = new LoggerConsole();
            m_devices = new SafeList<ControllableDevice>();
        }

        public void SetUserConnection(UserConnection connection)
        {
            RemoveUserConnection(m_userConnection);
            connection.OnDisconnect += RemoveUserConnection;
            m_userConnection = connection;
        }

        private void RemoveUserConnection(object sender)
        {
            if (m_userConnection == null)
                return;

            m_userConnection.OnDisconnect -= RemoveUserConnection;
            m_userConnection = null;
        }

        public void AddDevice(ControllableDevice device)
        {
            device.OnDeviceDisconnect += DeviceDisconnecting;
            device.OnCommandReceived += HandleDeviceReply;
            m_devices.Add(device);
        }

        public void RemoveDevice(ControllableDevice device)
        {
            m_devices.Remove(device);
            device.OnCommandReceived -= HandleDeviceReply;
            device.OnDeviceDisconnect -= DeviceDisconnecting;
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

            Log.Log("Received reply \"{0}\" from device {1} for user \"{2}\".", reply, device.DeviceID, UserData.Username);

            if (UserConnected)
                m_userConnection.Send(string.Format("Device {0} - {1}", device.DeviceID, reply));
        }

        private void DeviceDisconnecting(object sender, EventArgs e)
        {
            ControllableDevice device = sender as ControllableDevice;
            RemoveDevice(device);

            Log.Log("Device {0} disconnected from user \"{1}\".", device.DeviceID, UserData.Username);
        }
    }

    class UserData
    {
        public string Username { get; private set; }
        public byte[] PasswordHash { get; private set; }
        public byte[] Salt { get; private set; }

        public UserData(string userName, byte[] passwordHash, byte[] salt)
        {
            Username = userName;
            PasswordHash = passwordHash;
            Salt = salt;
        }
    }
}
