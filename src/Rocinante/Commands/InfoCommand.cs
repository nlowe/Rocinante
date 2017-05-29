using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using Rocinante.Types;

namespace Rocinante.Commands
{
    public class InfoCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name => "info";

        public string Description => "Prints details about the installation and site";

        public string Help => string.Empty;

        public void Execute(string[] args, IPublishContext ctx)
        {
            var sb = new StringBuilder($"\nRocinante Version {0}").AppendLine();

            sb.AppendLine($"Running on {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture} (Framework {RuntimeInformation.FrameworkDescription})")
              .AppendLine("RegisteredCommands: ");

            foreach(var cmd in ctx.Commands)
            {
                sb.AppendLine($"\t{cmd.Name}: {cmd.Description} (Provided by {cmd.GetType().FullName})");
            }

            sb.AppendLine().AppendLine("Registered Content Engines:");
            
            foreach(var engine in ctx.ContentEngines)
            {
                sb.AppendLine($"\t{engine.GetType().FullName}");
            }

            sb.AppendLine().AppendLine("Registered Theme Resolvers:");
            foreach(var resolver in ctx.ThemeResolvers)
            {
                sb.AppendLine($"\t{resolver.GetType().FullName}");
            }

            if(File.Exists("site.json"))
            {
                sb.AppendLine().AppendLine("Site Definition:")
                  .AppendLine(File.ReadAllText("site.json"));
            }

            Log.Info(sb.ToString());
        }
    }
}