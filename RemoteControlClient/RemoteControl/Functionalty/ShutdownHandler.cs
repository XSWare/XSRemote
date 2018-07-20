using System.Diagnostics;
using XSLibrary.Utility;

namespace RemoteShutdown.Functionalty
{
    class ShutdownHandler : CommandHandler
    {
        public void Shutdown(int seconds)
        {
            Logger.Log(LogLevel.Priority, GenerateInfo("Shutting down", seconds));
            string commandString = string.Format("/s /t {0}", seconds);
            StartShutdownProcess(commandString);
        }

        public void Restart(int seconds)
        {
            Logger.Log(LogLevel.Priority, GenerateInfo("Restarting", seconds));
            string commandString = string.Format("/r /t {0}", seconds);
            StartShutdownProcess(commandString);
        }

        private string GenerateInfo(string phrase, int seconds)
        {
            return phrase + (seconds > 0 ? (" in " + seconds + " seconds") : "") + ".";
        }

        public void AbortShutdown()
        {
            Logger.Log(LogLevel.Priority, "Shutdown aborted.");
            string commandString = string.Format("/a");
            StartShutdownProcess(commandString);
        }

        private void StartShutdownProcess(string commandString)
        {
            var psi = new ProcessStartInfo("shutdown", commandString);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
    }
}
