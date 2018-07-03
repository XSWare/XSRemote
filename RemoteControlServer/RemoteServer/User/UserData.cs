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

        public UserAccount(UserData userData)
        {
            UserData = userData;

            Log = new LoggerConsole();
            m_devices = new SafeList<ControllableDevice>();
        }

        public void AddDevice(ControllableDevice device)
        {
            device.OnDeviceDisconnect += DeviceDisconnecting;
            m_devices.Add(device);
        }

        public void RemoveDevice(ControllableDevice device)
        {
            m_devices.Remove(device);
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

        private void DeviceDisconnecting(object sender, EventArgs e)
        {
            RemoveDevice(sender as ControllableDevice);

            Log.Log("Device disconnected from user \"{0}\".", UserData.Username);
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
