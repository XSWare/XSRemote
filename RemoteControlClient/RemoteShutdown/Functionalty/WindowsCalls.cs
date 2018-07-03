using System;
using System.Runtime.InteropServices;

namespace RemoteShutdown
{
    class WindowsCalls
    {
        public const int WM_COMMAND = 0x0111;
        public const int WM_APPCOMMAND = 0x319;
        public const int WM_APP = 0x8000;

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        internal static void SendMessageW(IntPtr intPtr, int wM_APPCOMMAND, IntPtr message, object zero)
        {
            throw new NotImplementedException();
        }
    }
}
