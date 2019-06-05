using RemoteServer.Accounts;
using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.MultithreadingPatterns.Actor;
using XSLibrary.Utility;

namespace RemoteServer
{
    class CommandQueue : Actor<string>
    {
        IUserDataBase DataBase { get; set; }
        public AdminPool AdminPool { get; set; } = null;
        public UserPool UserPool { get; set; } = null;

        public CommandQueue(IUserDataBase database, Logger logger)
            : base(logger, "Command queue")
        {
            DataBase = database;
        }

        protected override void HandleMessage(string message)
        {
            AccountCommand(message);
        }

        private void AccountCommand(string cmd)
        {
            string[] cmdSplit = cmd.Split(' ');
            if (cmdSplit.Length < 2)
                return;

            string selection = cmdSplit[1];

            if (selection == "adduser" && cmdSplit.Length == 5)
                AddUser(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
            else if (selection == "addadmin" && cmdSplit.Length == 5)
                AddAdmin(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
            else if (selection == "remove" && cmdSplit.Length == 3)
                DeleteAccount(cmdSplit[2]);
            else if (selection == "changepw" && cmdSplit.Length == 5)
                ChangePassword(cmdSplit[2], cmdSplit[3], cmdSplit[4]);
            else if (selection == "kick" && cmdSplit.Length == 3)
                KickUser(cmdSplit[2]);
        }

        private void AddUser(string username, string contact, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 5, contact);
            if (DataBase.AddAccount(creationData))
                Logger.Log(LogLevel.Priority, "Added user \"{0}\" to database.", username);
        }

        private void AddAdmin(string username, string contact, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 0, contact);
            if (DataBase.AddAccount(creationData))
                Logger.Log(LogLevel.Priority, "Added admin \"{0}\" to database.", username);
        }

        private void DeleteAccount(string username)
        {
            if (DataBase.EraseAccount(username))
            {
                Logger.Log(LogLevel.Priority, "Removed account \"{0}\" from database.", username);
                KickUser(username);
            }
        }

        private void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (DataBase.ChangePassword(username, Encoding.ASCII.GetBytes(oldPassword), Encoding.ASCII.GetBytes(newPassword)))
            {
                Logger.Log(LogLevel.Priority, "Changed password for user \"{0}\".", username);
                KickUser(username);
            }
        }

        private void KickUser(string username)
        {
            if (AdminPool != null)
            {
                AdminAccount admin = AdminPool.GetElement(username);
                admin.Disconnect();
                AdminPool.ReleaseElement(username);
            }

            if (UserPool != null)
            {
                UserAccount user = UserPool.GetElement(username);
                user.Disconnect();
                UserPool.ReleaseElement(username);
            }

            Logger.Log(LogLevel.Priority, "Kicked \"{0}\" from the server.", username);
        }
    }
}
