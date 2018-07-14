using RemoteServer.Connections;
using RemoteServer.Device;
using System.Net;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.Utility;

namespace RemoteServer.User
{
    class UserAccount
    {
        public Logger Log { get; set; }
        public string Username { get; private set; }
        SafeList<ControllableDevice> m_devices;
        UserConnection m_userConnection;

        public bool UserConnected { get { return m_userConnection != null; } }

        public UserAccount(string username)
        {
            Username = username;

            Log = new LoggerConsole();
            m_devices = new SafeList<ControllableDevice>();
        }

        public void SetUserConnection(UserConnection connection)
        {
            Log.Log("User connected to account \"{0}\".", Username);

            RemoveUserConnection();
            connection.OnDisconnect += HandleUserDisconnect;
            m_userConnection = connection;
        }

        private void HandleUserDisconnect(object sender, EndPoint remote)
        {
            Log.Log("User disconnected from account \"{0}\".", Username);
            RemoveUserConnection();
        }

        private void RemoveUserConnection()
        {
            if (m_userConnection == null)
                return;

            m_userConnection.OnDisconnect -= HandleUserDisconnect;
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

            Log.Log("Received reply \"{0}\" from device {1} for user \"{2}\".", reply, device.DeviceID, Username);

            if (UserConnected)
                m_userConnection.Send(string.Format("Device {0} - {1}", device.DeviceID, reply));
        }

        private void DeviceDisconnecting(object sender, EndPoint remote)
        {
            ControllableDevice device = sender as ControllableDevice;
            RemoveDevice(device);

            Log.Log("Device {0} disconnected from user \"{1}\".", device.DeviceID, Username);
        }
    }
}
