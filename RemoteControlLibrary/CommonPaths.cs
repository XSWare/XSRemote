using System;

namespace RemoteShutdown
{
    public class CommonPaths
    {
        public static string APP_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\XSRemote\\";
        public const string ACCOUNT_FILE = "UserDataBase.mdf";
        public static string DATA_BASE_PATH = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + APP_FOLDER + ACCOUNT_FILE + ";Integrated Security=True";
    }
}
