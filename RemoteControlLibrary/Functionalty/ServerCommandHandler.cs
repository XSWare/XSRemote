using XSLibrary.Utility;

namespace RemoteShutdown.Functionalty
{
    public class ServerCommandHandler : CommandHandler
    {
        public void DisplayServerMessage(string message)
        {
            Logger.Log(LogLevel.Priority, message);
        }
    }
}
