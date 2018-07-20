using System;
using System.Diagnostics;

namespace RemoteShutdown
{
    class ShutdownHandler
    {
        public void Shutdown(int seconds)
        {
            Console.Out.WriteLine(GenerateInfo("Shutting down", seconds));
            string commandString = string.Format("/s /t {0}", seconds);
            StartShutdownProcess(commandString);
        }

        public void Restart(int seconds)
        {
            Console.Out.WriteLine(GenerateInfo("Restarting", seconds));
            string commandString = string.Format("/r /t {0}", seconds);
            StartShutdownProcess(commandString);
        }

        private string GenerateInfo(string phrase, int seconds)
        {
            return phrase + (seconds > 0 ? (" in " + seconds + " seconds") : "") + ".";
        }

        public void AbortShutdown()
        {
            Console.Out.WriteLine("Shutdown aborted.");
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
