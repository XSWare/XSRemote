using RemoteServer.Device.Modules;
using System.Collections.Generic;
using XSLibrary.Network.Connections;

namespace RemoteServer.Connections
{
    class DeviceConnection : ConnectionBase
    {
        public DeviceConnection(IConnection connection) : base (connection)
        {
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
