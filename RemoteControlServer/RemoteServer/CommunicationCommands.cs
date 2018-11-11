using RemoteServer.Accounts;
using System;
using XSLibrary.Utility;

namespace RemoteServer
{
    class CommunicationCommands
    {
        public Logger Logger { get; set; } = Logger.NoLog;

        UserPool UserPool { get; set; }

        public CommunicationCommands(UserPool userPool)
        {
            UserPool = userPool;
        }

        public void ManualCommand(string command)
        {
            string[] cmdSplit = command.Split(' ');

            if (cmdSplit.Length < 3)
            {
                Logger.Log(LogLevel.Priority, "Invalid command structure.");
                return;
            }

            UserAccount user = UserPool.GetElement(cmdSplit[0]);

            string deviceCommand = cmdSplit[2];
            for (int i = 3; i < cmdSplit.Length; i++)
            {
                deviceCommand += " " + cmdSplit[i];
            }

            if (cmdSplit[1] == "all" || !Int32.TryParse(cmdSplit[1], out int deviceID))
            {
                Logger.Log(LogLevel.Priority, "Sending command \"{0}\" to all devices of user \"{1}\"", deviceCommand, user.Username);
                user.BroadCastCommand(deviceCommand);
            }
            else
            {
                Logger.Log(LogLevel.Priority, "Sending command \"{0}\" to device \"{1}\" of user \"{2}\"", deviceCommand, deviceID.ToString(), user.Username);
                user.SendCommand(deviceID, deviceCommand);
            }

            UserPool.ReleaseElement(user.ID);
        }
    }
}
