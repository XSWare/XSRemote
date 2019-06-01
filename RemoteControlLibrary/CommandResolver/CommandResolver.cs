using System;

namespace RemoteShutdown.CommandResolving
{
    public interface ICommandResolver
    {
        string KeyPhrase { get; }
        bool ResolveCommand(string command, string[] arguments);
    }

    public abstract class SingleArgumentResolver : ICommandResolver
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

    public abstract class MultiArgumentResolver : ICommandResolver
    {
        public abstract string KeyPhrase { get; }

        public bool ResolveCommand(string command, string[] arguments)
        {
            if (command != KeyPhrase)
                return false;

            if (arguments.Length < 1)
                return false;

            string option = arguments[0];

            int argumentCount = arguments.Length - 1;
            string[] commandArguments = new string[argumentCount];
            Array.Copy(arguments, 1, commandArguments, 0, argumentCount);

            return Execute(option, commandArguments);
        }

        protected abstract bool Execute(string option, string[] arguments);
    }
}
