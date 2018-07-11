using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using RemoteServer.User;
using RemoteServer.Device;
using RemoteServer.Connections;
using XSLibrary.Cryptography.ConnectionCryptos;

namespace RemoteServer.Registrations
{
    class DeviceRegistration : Registration
    {
        int m_currentDeviceID;

        public DeviceRegistration(TCPAccepter accepter)
            : base(accepter)
        {
            m_currentDeviceID = 0;
        }

        protected override void HandleVerifiedConnection(UserAccount user, ConnectionInterface connection)
        {
            DeviceConnection deviceConnection = new DeviceConnection(connection);
            ControllableDevice device = new ControllableDevice(deviceConnection, m_currentDeviceID++);
            user.AddDevice(device);

            Logger.Log("Added device {0} to user \"{1}\".", device.DeviceID, user.UserData.Username);
            deviceConnection.Initialize();
        }
    }
}
