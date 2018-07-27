using RemoteServer.Registrations;
using RemoteServer.User;
using System;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Utility;

namespace RemoteServer
{
    class Program
    {
        static Logger logger;
        static FileUserBase dataBase = new FileUserBase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\", "accounts.txt");
        static UserPool accounts = new UserPool();
        static DeviceRegistration deviceRegistration;
        static UserRegistration userRegistration;

        static void Main(string[] args)
        {
#if DEBUG
            Logger.DefaultLogLevel = LogLevel.Detail;
#else
            Logger.DefaultLogLevel = LogLevel.Warning;
#endif
            logger = new LoggerConsole();

            logger.Log(LogLevel.Priority, "Server started.");

            GuardedAcceptor deviceAccepter = new GuardedAcceptor(22222, 1000);
            deviceRegistration = new DeviceRegistration(deviceAccepter, accounts, dataBase);
            deviceRegistration.Logger = logger;
            deviceRegistration.Run();

            GuardedAcceptor userAccepter = new GuardedAcceptor(22223, 1000);
            userRegistration = new UserRegistration(userAccepter, accounts, dataBase);
            userRegistration.Logger = logger;
            userRegistration.Run();

            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd == "backlog")
                    logger.Log(LogLevel.Priority, "Current backlog count: {0}", deviceRegistration);
                else if (cmd.Length > 7 && cmd.Substring(0, 7) == "account")
                    AccountCommand(cmd);
                else
                    ManualCommand(cmd);
            }

            logger.Log(LogLevel.Priority, "Shutting down server...");

            deviceRegistration.Dispose();
            userRegistration.Dispose();

            logger.Log(LogLevel.Priority, "Server shut down.");
            Console.In.Read();
        }

        private static void AccountCommand(string cmd)
        {
            string[] cmdSplit = cmd.Split(' ');
            if (cmdSplit.Length < 2)
                return;

            string selection = cmdSplit[1];

            if (selection == "adduser" && cmdSplit.Length == 4)
                AddUser(cmdSplit[2], cmdSplit[3]);
            else if (selection == "addadmin" && cmdSplit.Length == 4)
                AddAdmin(cmdSplit[2], cmdSplit[3]);
            else if (selection == "remove" && cmdSplit.Length == 3)
                DeleteAccount(cmdSplit[2]);
            else if (selection == "changepw" && cmdSplit.Length == 5)
                ChangePassword(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
        }

        private static void AddUser(string username, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 5);
            if (dataBase.AddAccount(creationData))
                logger.Log(LogLevel.Priority, "Added user \"{0}\" to database.", username);
        }

        private static void AddAdmin(string username, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 0);
            if (dataBase.AddAccount(creationData))
                logger.Log(LogLevel.Priority, "Added admin \"{0}\" to database.", username);
        }

        private static void DeleteAccount(string username)
        {
            if (dataBase.EraseAccount(username))
                logger.Log(LogLevel.Priority, "Removed account \"{0}\" from database.", username);
        }

        private static void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (dataBase.ChangePassword(username, Encoding.ASCII.GetBytes(oldPassword), Encoding.ASCII.GetBytes(newPassword)))
                logger.Log(LogLevel.Priority, "Changed password for user \"{0}\".", username);
        }

        static void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
                return;

            UserAccount user = accounts.GetElement(cmdSplit[0]);

            string deviceCommand = cmdSplit[2];
            for (int i = 3; i < cmdSplit.Length; i++)
            {
                deviceCommand += " " + cmdSplit[i];
            }
            
            if (cmdSplit[1] == "all" || !Int32.TryParse(cmdSplit[1], out int deviceID))
            {
                logger.Log(LogLevel.Priority, "Sending command \"{0}\" to all devices of user \"{1}\"", deviceCommand, user.Username);
                user.BroadCastCommand(deviceCommand);
            }
            else
            {
                logger.Log(LogLevel.Priority, "Sending command \"{0}\" to device \"{1}\" of user \"{2}\"", deviceCommand, deviceID.ToString(), user.Username);
                user.SendCommand(deviceID, deviceCommand);
            }

            accounts.ReleaseElement(user.ID);
        }
    }
}