using System;
using XSLibrary.Utility;

namespace RemoteShutdown.Functionalty
{
    public class MediaPlayerHandler : CommandHandler
    {
        private const int MESSAGE_PLAY = 0x00004978;
        private const int MESSAGE_NEXT = 0x0000497B;
        private const int MESSAGE_PREVIOUS = 0x0000497A;


        public void StartStop()
        {
            Logger.Log(LogLevel.Priority, "Resuming/pausing song.");
            SendMediaMessage(MESSAGE_PLAY);
        }

        public void Next()
        {
            Logger.Log(LogLevel.Priority, "Playing next song.");
            SendMediaMessage(MESSAGE_NEXT);
        }

        public void Previous()
        {
            Logger.Log(LogLevel.Priority, "Playing previous song.");
            SendMediaMessage(MESSAGE_PREVIOUS);
        }

        private void SendMediaMessage(int message)
        {
            WindowsCalls.SendMessageW(
                GetMediaPlayerWindowHandle(),
                WindowsCalls.WM_COMMAND,
                (IntPtr)message,
                IntPtr.Zero);
        }

        private IntPtr GetMediaPlayerWindowHandle()
        {
            return WindowsCalls.FindWindow("WMPlayerApp", "Windows Media Player");
        }
    }
}
