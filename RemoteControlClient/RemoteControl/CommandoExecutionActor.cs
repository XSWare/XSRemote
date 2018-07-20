using System;
using XSLibrary.MultithreadingPatterns.Actor;
using RemoteShutdown.CommandResolving;
using System.Collections.Generic;

namespace RemoteShutdown
{
    class CommandoExecutionActor : Actor<string>
    {
        List<CommandResolver> m_commandResolvers;

        public CommandoExecutionActor(List<CommandResolver> commandResolvers)
        {
            m_commandResolvers = commandResolvers;
        }

        protected override void HandleMessage(string command)
        {
            if (!ExecuteCommand(command))
                Console.Out.WriteLine("Unrecognized command.");
        }

        private bool ExecuteCommand(string command)
        {
            string[] commandParts = command.Split(' ');

            if (commandParts.Length < 1)
                return false;

            string keyPhrase = commandParts[0];

            int argumentCount = commandParts.Length - 1;
            string[] commandArguments = new string[argumentCount];
            Array.Copy(commandParts, 1, commandArguments, 0, argumentCount);

            foreach(CommandResolver resolver in m_commandResolvers)
            {
                if (resolver.ResolveCommand(keyPhrase, commandArguments))
                    return true;
            }

            return false;
        }
    }
}
