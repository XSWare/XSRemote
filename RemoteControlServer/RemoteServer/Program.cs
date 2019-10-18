using RemoteServer.Registrations;
using RemoteServer.Accounts;
using System;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Utility;
using RemoteShutdown;
using System.IO;

namespace RemoteServer
{
    class Program
    {
        static MultiLogger logger;

        static IUserDataBase dataBase;
        static UserPool users = new UserPool();
        static DeviceRegistration deviceRegistration;
        static UserRegistration userRegistration;
        static AdminRegistration adminRegistration;
        static CommandQueue accountCommands;
        static CommunicationCommands communicationCommands;

        static void Main(string[] args)
        {
#if DEBUG
            Logger.DefaultLogLevel = LogLevel.Detail;
#else
            Logger.DefaultLogLevel = LogLevel.Warning;
#endif
            logger = new MultiLogger();
            logger.Logs.Add(new LoggerConsole());
            logger.Logs.Add(new FileLogger(CommonPaths.APP_FOLDER + "log.txt"));

            logger.Log(LogLevel.Priority, "Server started.");

            if (!InitializeDatabase())
            {
                logger.Dispose();
                Console.In.Read();
                return;
            }

            accountCommands = new CommandQueue(dataBase, logger);
            AdminPool admins = new AdminPool(accountCommands, logger);
            accountCommands.AdminPool = admins;
            accountCommands.UserPool = users;

            communicationCommands = new CommunicationCommands(users);
            communicationCommands.Logger = logger;

            GuardedAcceptor deviceAccepter = new GuardedAcceptor(22222, 1000);
            deviceRegistration = new DeviceRegistration(deviceAccepter, users, dataBase);
            deviceRegistration.Logger = logger;
            deviceRegistration.Run();

            GuardedAcceptor userAccepter = new GuardedAcceptor(22223, 1000);
            userRegistration = new UserRegistration(userAccepter, users, dataBase);
            userRegistration.Logger = logger;
            userRegistration.Run();

            GuardedAcceptor adminAccepter = new GuardedAcceptor(22224, 3);
            adminRegistration = new AdminRegistration(adminAccepter, admins, dataBase);
            adminRegistration.Logger = logger;
            adminRegistration.Run();

            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd.Length > 7 && cmd.Substring(0, 7) == "account")
                    accountCommands.SendMessage(cmd);
                else
                    communicationCommands.ManualCommand(cmd);
            }

            logger.Log(LogLevel.Priority, "Shutting down server...");

            deviceRegistration.Dispose();
            userRegistration.Dispose();
            adminRegistration.Dispose();
            accountCommands.Stop(true);

            logger.Log(LogLevel.Priority, "Server shut down.");
            logger.Dispose();
            Console.In.Read();
        }

        static bool InitializeDatabase()
        {
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