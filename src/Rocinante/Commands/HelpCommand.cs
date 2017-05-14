using System;
using NLog;
using Rocinante.Types;
using Rocinante.Types.Extensions;

namespace Rocinante.Commands
{
    public class HelpCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name { get; } = "Help";

        public string Description { get; } = "Prints help information for commands";

        public string Help { get; } = string.Empty;

        private readonly CommandMap RegisteredCommands;

        public HelpCommand(CommandMap registeredCommands)
        {
            RegisteredCommands = registeredCommands;
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Log.Info("Supported Commands:");

                foreach(var cmd in RegisteredCommands.Values)
                {
                    Log.Info("\t{0}: {1}", cmd.Name, cmd.Description);
                }
            }
            else
            {
                ICommand cmd;
                if(!RegisteredCommands.TryGetValue(args[0], out cmd))
                {
                    Log.Fatal("Unknown command: {0}", args[0]);
                    Environment.Exit(-1);
                }

                Log.Info("{0}: {1}", cmd.Name, cmd.Description);
                if(!cmd.Help.IsNullOrEmpty())
                {
                    Log.Info("\n{0}", cmd.Help);
                }
            }
        }
    }
}