using System;

namespace RemoteShutdown
{
    class MediaPlayerHandler
    {
        private const int APP_MESSAGE_PLAY_PAUSE = 0x00004978;
        private const int MESSAGE_MEDIA_TRACK_SELECTION = 0xC;
        private const int MESSAGE_NEXT = 0xB0000;
        private const int MESSAGE_PREVIOUS = 0xC0000;


        public void StartStop()
        {
            WindowsCalls.SendMessageW(GetMediaPlayerWindowHandle(), WindowsCalls.WM_COMMAND, (IntPtr)APP_MESSAGE_PLAY_PAUSE, IntPtr.Zero);
        }

        public void Next()
        {
            WindowsCalls.PostMessage(GetMediaPlayerWindowHandle(), 0xC02B, (IntPtr)MESSAGE_MEDIA_TRACK_SELECTION, (IntPtr)MESSAGE_NEXT);
            WindowsCalls.SendMessageW(GetMediaPlayerWindowHandle(), 0x8002, (IntPtr)0, (IntPtr)0);
        }

        public void Previous()
        {
            WindowsCalls.PostMessage(GetMediaPlayerWindowHandle(), 0xC02B, (IntPtr)MESSAGE_MEDIA_TRACK_SELECTION, (IntPtr)MESSAGE_PREVIOUS);
            WindowsCalls.SendMessageW(GetMediaPlayerWindowHandle(), 0x8002, (IntPtr)0, (IntPtr)0);
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
