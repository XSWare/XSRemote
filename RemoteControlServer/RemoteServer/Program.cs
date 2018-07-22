using RemoteServer.Registrations;
using RemoteServer.User;
using System;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Network.Acceptors;
using XSLibrary.Utility;

namespace RemoteServer
{
    class Program
    {
        static Logger logger;
        static FileUserBase dataBase = new FileUserBase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\", "accounts.txt");
        static AccountPool accounts = new AccountPool();
        static DeviceRegistration deviceRegistration = new DeviceRegistration(new GuardedAcceptor(22222, 1000), dataBase, accounts);
        static UserRegistration userRegistration = new UserRegistration(new GuardedAcceptor(22223, 1000), dataBase, accounts);

        static void Main(string[] args)
        {
#if DEBUG
            Logger.DefaultLogLevel = LogLevel.Detail;
#endif
            logger = new LoggerConsole();

            deviceRegistration.Logger = logger;
            deviceRegistration.Run();

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

            deviceRegistration.Dispose();
            userRegistration.Dispose();
        }

        private static void AccountCommand(string cmd)
        {
            string[] cmdSplit = cmd.Split(' ');
            if (cmdSplit.Length < 2)
                return;

            string selection = cmdSplit[1];

            if (selection == "add" && cmdSplit.Length == 4)
                userRegistration.AddUser(cmdSplit[2], cmdSplit[3]);
            else if (selection == "remove" && cmdSplit.Length == 3)
                userRegistration.DeleteUser(cmdSplit[2]);
            else if (selection == "changepw" && cmdSplit.Length == 5)
                userRegistration.ChangePassword(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
        }

        static void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
                return;

            UserAccount user = accounts.GetAccount(cmdSplit[0]);

            if (!user.StillInUse())
            {
                accounts.DisposeAccount(user);
                return;
            }

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
        }
    }
}