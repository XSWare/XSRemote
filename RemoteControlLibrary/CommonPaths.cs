using System;

namespace RemoteShutdown
{
    public class CommonPaths
    {
        public static string ACCOUNT_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\RemoteControl\\";
        public static string ACCOUNT_FILE = "accounts.txt";
    }
}
