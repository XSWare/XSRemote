using XSLibrary.Network.Connections;
using RemoteServer.Accounts;
using RemoteServer.Device;
using RemoteServer.Connections;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;

namespace RemoteServer.Registrations
{
    class DeviceRegistration : Registration<UserAccount>
    {
        int m_currentDeviceID;

        public DeviceRegistration(TCPAcceptor accepter, UserPool accounts, IUserDataBase dataBase)
            : base(accepter, accounts, dataBase)
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
