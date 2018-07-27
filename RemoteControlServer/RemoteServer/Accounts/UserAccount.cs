using RemoteServer.Connections;
using RemoteServer.Device;
using System.Net;
using XSLibrary.Network.Registrations;
using XSLibrary.ThreadSafety.Containers;
using XSLibrary.ThreadSafety.Executors;
using XSLibrary.Utility;

namespace RemoteServer.Accounts
{
    class UserAccount: IUserAccount
    {
        SafeList<ControllableDevice> m_devices;
        SafeExecutor m_userLock = new SingleThreadExecutor();
        UserConnection m_userConnection;

        public bool UserConnected { get { return m_userConnection != null; } }

        public UserAccount(string username) : base(username)
        {
            m_devices = new SafeList<ControllableDevice>();
        }

        public bool SetUserConnection(UserConnection connection)
        {
            return m_userLock.Execute(() =>
            {
                if (m_userConnection != null)
                {
                    Logger.Log(LogLevel.Priority, "User tried to connect to account \"{0}\" but it already has a user connection.", Username);
                    return false;
                }

                Logger.Log(LogLevel.Priority, "User connected to account \"{0}\".", Username);
                m_userConnection = connection;
                return true;
            });
        }

        private void RemoveUserConnection()
        {
            m_userLock.Execute(() =>
            {
                if (m_userConnection == null)
                    return;

                Logger.Log(LogLevel.Priority, "User disconnected from account \"{0}\".", Username);
                m_userConnection.OnDisconnect -= HandleUserDisconnect;
                m_userConnection = null;
            });
        }

        public void AddDevice(ControllableDevice device)
        {
            Logger.Log(LogLevel.Priority, "Added device {0} to user \"{1}\".", device.DeviceID, Username);
            m_devices.Add(device);
            device.OnCommandReceived += HandleDeviceReply;
            device.OnDeviceDisconnect += DeviceDisconnecting;
        }

        public void RemoveDevice(ControllableDevice device)
        {
            device.OnDeviceDisconnect -= DeviceDisconnecting;
            device.OnCommandReceived -= HandleDeviceReply;
            m_devices.Remove(device);
            Logger.Log(LogLevel.Priority, "Device {0} disconnected from user \"{1}\".", device.DeviceID, Username);
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

            m_userLock.Execute(() =>
            {
                if (UserConnected)
                    m_userConnection.Send(string.Format("Device {0} - {1}", device.DeviceID, reply));
            });
        }

        private void DeviceDisconnecting(object sender, EndPoint remote)
        {
            RemoveDevice(sender as ControllableDevice);
        }

        public void HandleUserDisconnect(object sender, EndPoint remote)
        {
            RemoveUserConnection();
        }

        public override void Dispose()
        {
        }
    }
}
