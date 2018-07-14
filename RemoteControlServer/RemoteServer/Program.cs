using RemoteServer.Registrations;
using RemoteServer.User;
using System;
using XSLibrary.Network.Accepters;

namespace RemoteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            DeviceRegistration deviceRegistration = new DeviceRegistration(new TCPAccepter(22222, 1000));
            UserRegistration userRegistration = new UserRegistration(new TCPAccepter(22223, 1000));

            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd == "backlog")
                    Console.Out.WriteLine("Current backlog count: {0}", deviceRegistration);
                else if (cmd.Substring(0, 7) == "adduser")
                    AddUser(cmd, userRegistration);
                else
                    ManualCommand(cmd);
            }
        }

        private static void AddUser(string cmd, UserRegistration registration)
        {
            string[] cmdSplit = cmd.Split(' ');
            if (cmdSplit.Length != 3)
                return;

            registration.AddUser(cmdSplit[1], cmdSplit[2]);
        }

        static void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
                return;

            UserAccount user = DataBase.Instance.GetAccount(cmdSplit[0]);

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