using System;
using XSLibrary.Utility;

namespace RemoteShutdown.Functionalty
{
    public class VolumeHandler : CommandHandler
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;

        private IntPtr GetWindowHandle()
        {
            return WindowsCalls.FindWindowByCaption(IntPtr.Zero, "Remote");
        }

        private void SendVolumeMessage(int message)
        {
            WindowsCalls.SendMessageW(
                GetWindowHandle(),
                WindowsCalls.WM_APPCOMMAND, 
                GetWindowHandle(), 
                (IntPtr)message);
        }

        public void Mute()
        {
            Logger.Log(LogLevel.Priority, "Muting/unmuting audio.");
            SendVolumeMessage(APPCOMMAND_VOLUME_MUTE);
        }

        public void VolDown()
        {
            Logger.Log(LogLevel.Priority, "Lowering volume.");
            SendVolumeMessage(APPCOMMAND_VOLUME_DOWN);
        }

        public void VolUp()
        {
            Logger.Log(LogLevel.Priority, "Increasing volume.");
            SendVolumeMessage(APPCOMMAND_VOLUME_UP);
        }
    }
}
