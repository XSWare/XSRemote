﻿using RemoteServer.Authentications;
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
        int m_currentDeviceID;

        public DeviceRegistration(TCPAccepter accepter, KeyExchange keyExchange, Authentication authentication)
            : base(accepter, keyExchange, authentication)
        {
            m_currentDeviceID = 0;
        }

        protected override void HandleVerifiedConnection(UserAccount user, TCPPacketConnection clientConnection)
        {
            DeviceConnection deviceConnection = new DeviceConnection(clientConnection);
            ControllableDevice device = new ControllableDevice(deviceConnection, m_currentDeviceID++);
            user.AddDevice(device);

            Logger.Log("Added device {0} to user \"{1}\".", device.DeviceID, user.UserData.Username);
        }
    }
}
