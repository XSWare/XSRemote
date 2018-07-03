using RemoteServer.Authentications;
using XSLibrary.Network.Accepters;
using XSLibrary.Network.Connections;
using RemoteServer.User;
using RemoteServer.Device;
using RemoteServer.KeyExchanges;
using RemoteServer.Connections;

namespace RemoteServer.Registrations
{
    class DeviceRegistration : Registration
    {
        public DeviceRegistration(TCPAccepter accepter, KeyExchange keyExchange, Authentication authentication)
            : base(accepter, keyExchange, authentication)
        {
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPConnection clientConnection)
        {
            DeviceConnection deviceConnection = new DeviceConnection(clientConnection);
            ControllableDevice device = new ControllableDevice(deviceConnection);
            user.AddDevice(device);

            clientConnection.Logger.Log("Added device to user \"{0}\".", user.UserData.Username);
        }
    }
}
