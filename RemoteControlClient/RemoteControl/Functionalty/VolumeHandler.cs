using System;

namespace RemoteShutdown
{
    class VolumeHandler
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;

        private IntPtr GetConsoleWindowHandle()
        {
            Console.Title = "RemoteControl";
            return WindowsCalls.FindWindowByCaption(IntPtr.Zero, Console.Title);
        }

        private void SendVolumeMessage(int message)
        {
            WindowsCalls.SendMessageW(
                GetConsoleWindowHandle(),
                WindowsCalls.WM_APPCOMMAND, 
                GetConsoleWindowHandle(), 
                (IntPtr)message);
        }

        public void Mute()
        {
            SendVolumeMessage(APPCOMMAND_VOLUME_MUTE);
        }

        public void VolDown()
        {
            SendVolumeMessage(APPCOMMAND_VOLUME_DOWN);
        }

        public void VolUp()
        {
            SendVolumeMessage(APPCOMMAND_VOLUME_UP);
        }
    }
}
