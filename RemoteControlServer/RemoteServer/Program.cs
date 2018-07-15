using RemoteServer.Registrations;
using RemoteServer.User;
using System;
using XSLibrary.Network.Accepters;

namespace RemoteServer
{
    class Program
    {
        static DeviceRegistration deviceRegistration = new DeviceRegistration(new GuardedAccepter(22222, 1000));
        static UserRegistration userRegistration = new UserRegistration(new GuardedAccepter(22223, 1000));

        static void Main(string[] args)
        {
            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd == "backlog")
                    Console.Out.WriteLine("Current backlog count: {0}", deviceRegistration);
                else if (cmd.Length > 7 && cmd.Substring(0, 7) == "account")
                    AccountCommand(cmd);
                else
                    ManualCommand(cmd);
            }
        }

        private static void AccountCommand(string cmd)
        {
            string[] cmdSplit = cmd.Split(' ');
            if (cmdSplit.Length < 2)
                return;

            string selection = cmdSplit[1];

            if (selection == "add" && cmdSplit.Length == 4)
                userRegistration.AddUser(cmdSplit[2], cmdSplit[3]);
            else if (selection == "delete" && cmdSplit.Length == 3)
                userRegistration.DeleteUser(cmdSplit[2]);
            else if (selection == "changepw" && cmdSplit.Length == 5)
                userRegistration.ChangePassword(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
        }

        static void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
                return;

            UserAccount user = userRegistration.GetUserAccount(cmdSplit[0]);

            string deviceCommand = cmdSplit[2];
            for (int i = 3; i < cmdSplit.Length; i++)
            {
                deviceCommand += " " + cmdSplit[i];
            }
            
            if (cmdSplit[1] == "all" || !Int32.TryParse(cmdSplit[1], out int deviceID))
            {
                Console.Out.WriteLine("Sending command \"{0}\" to all devices of user \"{1}\"", deviceCommand, user.Username);
                user.BroadCastCommand(deviceCommand);
            }
            else
            {
                Console.Out.WriteLine("Sending command \"{0}\" to device \"{1}\" of user \"{2}\"", deviceCommand, deviceID.ToString(), user.Username);
                user.SendCommand(deviceID, deviceCommand);
            }
        }
    }
}