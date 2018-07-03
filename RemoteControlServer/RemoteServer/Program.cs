using RemoteServer.Authentications;
using RemoteServer.Connections;
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
            DeviceRegistration deviceRegistration = new DeviceRegistration(
                new TCPAccepter(22222, 1000),
                new KeyExchanges.DummyKeyExchange(),
                new DummyAuthentication());

            UserRegistration userRegistration = new UserRegistration(
                new TCPAccepter(22223, 1000),
                new KeyExchanges.DummyKeyExchange(),
                new DummyAuthentication());

            string cmd;
            while ((cmd = Console.In.ReadLine()) != "exit")
            {
                if (cmd == "backlog")
                    Console.Out.WriteLine("Current backlog count: {0}", deviceRegistration);
                else
                    ManualCommand(cmd);
            }
        }

        static void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
                return;

            UserAccount user = DummyDataBase.Instance.GetAccount(cmdSplit[0]);

            string deviceCommand = cmdSplit[2];
            for (int i = 3; i < cmdSplit.Length; i++)
            {
                deviceCommand += " " + cmdSplit[i];
            }
            
            if (cmdSplit[1] == "all" || !Int32.TryParse(cmdSplit[1], out int deviceID))
            {
                Console.Out.WriteLine("Sending command \"{0}\" to all devices of user \"{1}\"", deviceCommand, user.UserData.Username);
                user.BroadCastCommand(deviceCommand);
            }
            else
            {
                Console.Out.WriteLine("Sending command \"{0}\" to device \"{1}\" of user \"{2}\"", deviceCommand, deviceID.ToString(), user.UserData.Username);
                user.SendCommand(deviceID, deviceCommand);
            }
        }
    }
}