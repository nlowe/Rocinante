using System;
using System.IO;
using Rocinante.Types;
using Jil;
using NLog;

namespace Rocinante.Commands
{
    public class NewCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name => "new";

        public string Description => "Create a new site in the current directory";

        public string Help => @"
Initializes the current directory with the default site template:
* A folder for posts
* site.json: The configuration for the site
";

        public void Execute(string[] args, IPublishContext ctx)
        {
            var site = new Site();

            var root = Directory.GetCurrentDirectory();
            var posts = Path.Combine(root, site.PostSource);

            MakeDirectoryIfNotExists(posts);

            Log.Info("Creating default site configuration");
            using(var writer = new StreamWriter(new FileStream(Path.Combine(root, "site.json"), FileMode.OpenOrCreate)))
            {
                JSON.Serialize(site, writer, Options.ISO8601PrettyPrintExcludeNulls);
            }

            Log.Info("Site created at {0}", root);
        }

        private void MakeDirectoryIfNotExists(string path)
        {
            if(!Directory.Exists(path))
            {
                Log.Info("Creating posts directory");
                Directory.CreateDirectory(path);
            }
        }
    }
}