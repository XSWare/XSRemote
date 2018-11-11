using System.Text;
using XSLibrary.Cryptography.AccountManagement;
using XSLibrary.Utility;

namespace RemoteServer
{
    class AccountCommands
    {
        public Logger Logger { get; set; } = Logger.NoLog;

        IUserDataBase DataBase { get; set; }

        public AccountCommands(IUserDataBase database)
        {
            DataBase = database;
        }

        public void AccountCommand(string cmd)
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

        private void AddUser(string username, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 5);
            if (DataBase.AddAccount(creationData))
                Logger.Log(LogLevel.Priority, "Added user \"{0}\" to database.", username);
        }

        private void AddAdmin(string username, string password)
        {
            AccountCreationData creationData = new AccountCreationData(username, Encoding.ASCII.GetBytes(password), 0);
            if (DataBase.AddAccount(creationData))
                Logger.Log(LogLevel.Priority, "Added admin \"{0}\" to database.", username);
        }

        private void DeleteAccount(string username)
        {
            if (DataBase.EraseAccount(username))
                Logger.Log(LogLevel.Priority, "Removed account \"{0}\" from database.", username);
        }

        private void ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (DataBase.ChangePassword(username, Encoding.ASCII.GetBytes(oldPassword), Encoding.ASCII.GetBytes(newPassword)))
                Logger.Log(LogLevel.Priority, "Changed password for user \"{0}\".", username);
        }
    }
}
