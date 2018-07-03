using System.Collections.Generic;
using RemoteServer.Device.Modules;
using RemoteServer.Connections;
using System;

namespace RemoteServer.Device
{
    class ControllableDevice
    {
        public event EventHandler OnDeviceDisconnect;
        public event DeviceConnection.CommandReceivedHandler OnCommandReceived;

        public int DeviceID { get; private set; }
        DeviceConnection m_connection;
        List<DeviceModule> m_modules;

        public ControllableDevice(DeviceConnection connection, int deviceID)
        {
            DeviceID = deviceID;
            m_connection = connection;
            m_connection.OnDisconnect += HandleDisconnect;
            m_connection.OnCommandReceived += HandleCommandReceived;
            m_modules = m_connection.RequestModules();
        }

        public void SendCommand(string command)
        {
            DeviceModule targetModule = GetTargetModule(command);
            if (targetModule == null)
                return;

            m_connection.Logger.Log("Sending to device: {0}", command);

            m_connection.SendCommand(targetModule.TranslateCommand(command));
        }

        private void HandleCommandReceived(object sender, string command)
        {
            OnCommandReceived?.Invoke(this, command);
        }

        private void HandleDisconnect(object sender)
        {
            OnDeviceDisconnect.Invoke(this, new EventArgs());
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
