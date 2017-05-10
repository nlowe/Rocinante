using System;

namespace Rocinante.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get; } = "Help";

        public string Description { get; } = "Prints help information for commands";

        public string Help { get; } = string.Empty;

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Supported Commands:");
            }
            else
            {

            }
        }
    }
}