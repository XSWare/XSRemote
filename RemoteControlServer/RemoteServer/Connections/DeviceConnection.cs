using RemoteServer.Device.Modules;
using System.Collections.Generic;
using XSLibrary.Network.Connections;

namespace RemoteServer.Connections
{
    class DeviceConnection : ConnectionBase
    {
        public DeviceConnection(TCPConnection connection) : base (connection)
        {
        }

        protected override void ReceiveCommand(string command)
        {
            Logger.Log("Received data from device.");
        }

        public List<DeviceModule> RequestModules()
        {
            List<DeviceModule> deviceModules = new List<DeviceModule>
            {
                new RepeaterModule()
            };
            return deviceModules;
        }
    }
}
