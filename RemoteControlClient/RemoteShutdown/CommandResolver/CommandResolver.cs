namespace RemoteShutdown.CommandResolving
{
    public abstract class CommandResolver
    {
        public abstract string KeyPhrase { get; }

        public bool ResolveCommand(string command, string[] arguments)
        {
            if (command != KeyPhrase)
                return false;

            if (arguments.Length < 1)
                return false;

            string argument = "";
            string option = arguments[0];


            if (arguments.Length > 1)
                argument = arguments[1];

            return Execute(option, argument);
        }

        protected abstract bool Execute(string option, string argument);
    }
}
