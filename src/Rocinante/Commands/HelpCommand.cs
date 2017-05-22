using System;
using System.Linq;
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

        public void Execute(string[] args, IPublishContext ctx)
        {
            if (args.Length == 0)
            {
                Log.Info("Supported Commands:");

                foreach(var cmd in ctx.Commands)
                {
                    Log.Info("\t{0}: {1}", cmd.Name, cmd.Description);
                }
            }
            else
            {
                var cmd = ctx.Commands.FirstOrDefault(c => c.Name.Equals(args[0], StringComparison.CurrentCultureIgnoreCase));
                if(cmd == null)
                {
                    Log.Fatal("Unknown command {0}", args[0]);
                    Environment.Exit(ExitCodes.UNKNOWN_COMMAND);
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