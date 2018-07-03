using System;

namespace RemoteShutdown.CommandResolving
{
    class ShutdownCommandResolve : CommandResolver
    {
        ShutdownHandler m_shutdownHandler;

        public ShutdownCommandResolve(ShutdownHandler shutdownHandler)
        {
            m_shutdownHandler = shutdownHandler;
        }

        public override string KeyPhrase { get { return "control"; } }

        protected override bool Execute(string option, string argument)
        {
            int delay = 60;

            try { delay = ParseDelay(argument); }
            catch { delay = 60; }

            switch (option)
            {
                case "shutdown":
                    m_shutdownHandler.Shutdown(delay);
                    return true;

                case "restart":
                    m_shutdownHandler.Restart(delay);
                    return true;

                case "abort":
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
