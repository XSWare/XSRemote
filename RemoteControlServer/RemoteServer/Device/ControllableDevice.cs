using System.Collections.Generic;
using RemoteServer.Device.Modules;
using RemoteServer.Connections;
using System.Net;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteServer.Device
{
    class ControllableDevice
    {
        public event OnDisconnectEvent.EventHandle OnDeviceDisconnect
        {
            add { DisconnectHandle.Event += value; }
            remove { DisconnectHandle.Event -= value; }
        }

        public event ConnectionBase.DataReceivedHandler OnCommandReceived;

        public int DeviceID { get; private set; }
        DeviceConnection m_connection;
        List<DeviceModule> m_modules;

        OnDisconnectEvent DisconnectHandle { get; set; } = new OnDisconnectEvent();

        public ControllableDevice(DeviceConnection connection, int deviceID)
        {
            DeviceID = deviceID;
            m_connection = connection;
            m_connection.OnDisconnect += HandleDisconnect;
            m_connection.OnDataReceived += HandleCommandReceived;
            m_modules = m_connection.RequestModules();
        }

        public void SendCommand(string command)
        {
            DeviceModule targetModule = GetTargetModule(command);
            if (targetModule == null)
                return;

            m_connection.Logger.Log(LogLevel.Information, "Sending to device {0}: {1}", DeviceID, command);

            m_connection.Send(targetModule.TranslateCommand(command));
        }

        private void HandleCommandReceived(object sender, string command)
        {
            OnCommandReceived?.Invoke(this, command);
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            DisconnectHandle.Invoke(this, remote);
        }

        private DeviceModule GetTargetModule(string command)
        {
            foreach (DeviceModule module in m_modules)
            {
                if (module.IsModuleTargeted(command))
                    return module;
            }

            return null;
        }
    }
}
