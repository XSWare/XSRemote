using RemoteShutdownLibrary;
using RemoteShutdown.Functionalty;
using System;

namespace RemoteShutdown.CommandResolving
{
    public class ShutdownCommandResolve : SingleArgumentResolver
    {
        ShutdownHandler m_shutdownHandler;

        public ShutdownCommandResolve(ShutdownHandler shutdownHandler)
        {
            m_shutdownHandler = shutdownHandler;
        }

        public override string KeyPhrase { get { return Commands.CONTROL; } }

        protected override bool Execute(string option, string argument)
        {
            int delay = 60;

            try { delay = ParseDelay(argument) * 60; }
            catch { delay = 60; }

            switch (option)
            {
                case Commands.CONTROL_SHUTDOWN:
                    m_shutdownHandler.Shutdown(delay);
                    return true;

                case Commands.CONTROL_RESTART:
                    m_shutdownHandler.Restart(delay);
                    return true;

                case Commands.CONTROL_ABORT:
                    m_shutdownHandler.AbortShutdown();
                    return true;
            }

            return false;
        }

        private int ParseDelay(string delayString)
        {
            try
            {
                int delay = Convert.ToInt32(delayString);
                return delay;
            }
            catch
            {
                return 0;
            }
        }
    }
}
