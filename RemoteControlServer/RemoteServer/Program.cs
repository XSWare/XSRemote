using RemoteServer.Registrations;
using RemoteServer.Accounts;
using System;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Utility;

namespace RemoteServer
{
    class Program
    {
        static string dataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\";
        static MultiLogger logger;
        static FileUserBase dataBase = new FileUserBase(dataFolderPath, "accounts.txt");
        static UserPool users = new UserPool();
        static DeviceRegistration deviceRegistration;
        static UserRegistration userRegistration;
        static AdminRegistration adminRegistration;
        static AccountCommands accountCommands;
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
            logger.Logs.Add(new FileLogger(dataFolderPath + "log.txt"));

            logger.Log(LogLevel.Priority, "Server started.");

            accountCommands = new AccountCommands(dataBase);
            accountCommands.Logger = logger;

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

            AdminPool admins = new AdminPool(logger);
            GuardedAcceptor adminAccepter = new GuardedAcceptor(22224, 3);
            adminRegistration = new AdminRegistration(adminAccepter, admins, dataBase);
            adminRegistration.Logger = logger;
            adminRegistration.Run();

            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd.Length > 7 && cmd.Substring(0, 7) == "account")
                    accountCommands.AccountCommand(cmd);
                else
                    communicationCommands.ManualCommand(cmd);
            }

            logger.Log(LogLevel.Priority, "Shutting down server...");

            deviceRegistration.Dispose();
            userRegistration.Dispose();
            adminRegistration.Dispose();

            logger.Log(LogLevel.Priority, "Server shut down.");
            logger.Dispose();
            Console.In.Read();
        }
    }
}