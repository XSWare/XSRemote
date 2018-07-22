using XSLibrary.Network.Connections;
using RemoteServer.User;
using RemoteServer.Device;
using RemoteServer.Connections;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;

namespace RemoteServer.Registrations
{
    class DeviceRegistration : Registration
    {
        int m_currentDeviceID;

        public DeviceRegistration(TCPAcceptor accepter, IUserDataBase dataBase, AccountPool accounts)
            : base(accepter, dataBase, accounts)
        {
            m_currentDeviceID = 0;
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPPacketConnection connection)
        {
            DeviceConnection deviceConnection = new DeviceConnection(connection);
            ControllableDevice device = new ControllableDevice(deviceConnection, m_currentDeviceID++);
            user.AddDevice(device);

            connection.InitializeReceiving();
        }
    }
}
