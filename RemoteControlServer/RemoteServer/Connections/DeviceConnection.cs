using RemoteServer.Device.Modules;
using System.Collections.Generic;
using XSLibrary.Network.Connections;

namespace RemoteServer.Connections
{
    class DeviceConnection : ConnectionBase
    {
        public delegate void CommandReceivedHandler(object sender, string command);
        public event CommandReceivedHandler OnCommandReceived;

        public DeviceConnection(IConnection connection) : base (connection)
        {
        }

        protected override void ReceiveCommand(string command)
        {
            OnCommandReceived?.Invoke(this, command);
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
