using System.IO;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Utility;

namespace RemoteShutdown
{
    public class DatabaseHandling
    {
        public static bool InitializeDatabase(out IUserDataBase dataBase)
        {
            return InitializeDatabase(out dataBase, new NoLog());
        }

        public static bool InitializeDatabase(out IUserDataBase dataBase, Logger logger)
        {
            dataBase = null;

            if (!File.Exists(CommonPaths.DATABASE_FILEPATH))
            {
                logger.Log(LogLevel.Error, "Database in \"{0}\" not found. Copying local database...", CommonPaths.DATABASE_FILEPATH);

                try
                {
                    File.Copy(CommonPaths.DATABASE_FILENAME, CommonPaths.DATABASE_FILEPATH);
                    logger.Log(LogLevel.Information, "Local database copied to \"{0}\"", CommonPaths.DATABASE_FILEPATH);
                }
                catch
                {
                    logger.Log(LogLevel.Error, "Could not copy local database! Please create a database and restart the server.");
                    return false;
                }
            }

            dataBase = new ServiceUserBase(CommonPaths.DATABASE_CONNECTION_STRING);
            return true;
        }
    }
}
