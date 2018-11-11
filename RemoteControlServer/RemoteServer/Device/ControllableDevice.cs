using System.Collections.Generic;
using RemoteServer.Device.Modules;
using RemoteServer.Connections;
using System.Net;
using XSLibrary.Utility;
using XSLibrary.ThreadSafety.Events;

namespace RemoteServer.Device
{
    class ControllableDevice
    {
        public IEvent<ControllableDevice, EndPoint> OnDeviceDisconnect;
        public event ConnectionBase.DataReceivedHandler OnCommandReceived;

        public int DeviceID { get; private set; }
        DeviceConnection m_connection;
        List<DeviceModule> m_modules;

        public ControllableDevice(DeviceConnection connection, int deviceID)
        {
            DeviceID = deviceID;
            m_connection = connection;
            m_connection.OnDataReceived += HandleCommandReceived;
            OnDeviceDisconnect = m_connection.OnDisconnect.CreateRelay(this);
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

        private DeviceModule GetTargetModule(string command)
        {
            foreach (DeviceModule module in m_modules)
            {
                if (module.IsModuleTargeted(command))
                    return module;
            }

            return null;
        }

        public void Disconnect()
        {
            m_connection.Disconnect();
        }
    }
}
