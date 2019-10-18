using System;

namespace RemoteShutdown
{
    public class CommonPaths
    {
        public static string APP_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\XSRemote\\";
        public const string DATABASE_FILENAME = "UserDataBase.mdf";
        public static string DATABASE_FILEPATH = APP_FOLDER + DATABASE_FILENAME;
        public static string DATABASE_CONNECTION_STRING = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + APP_FOLDER + DATABASE_FILENAME + ";Integrated Security=True";
    }
}
