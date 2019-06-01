using RemoteShutdown.Functionalty;
using RemoteShutdownLibrary;

namespace RemoteShutdown.CommandResolving
{
    public class ServerCommandResolver : MultiArgumentResolver
    {
        public override string KeyPhrase { get { return Commands.SERVER; } }

        ServerCommandHandler m_commandHandler;

        public ServerCommandResolver(ServerCommandHandler commandHandler)
        {
            m_commandHandler = commandHandler;
        }

        protected override bool Execute(string option, string[] arguments)
        {
            if (option == Commands.SERVER_MESSAGE)
            {
                string message = arguments[0];
                for (int i = 1; i < arguments.Length; i++)
                    message += " " + arguments[i];

                m_commandHandler.DisplayServerMessage(message);
                return true;
            }

            return false;
        }
    }
}
