using System;
using System.IO;
using Rocinante.Types;
using Jil;

namespace Rocinante.Commands
{
    public class NewCommand : ICommand
    {
        public string Name => "new";

        public string Description => "Create a new site in the current directory";

        public string Help => string.Empty;

        public void Execute(string[] args)
        {
            var site = new Site();

            var root = Directory.GetCurrentDirectory();
            var posts = Path.Combine(root, site.PostSource);

            MakeDirectoryIfNotExists(posts);

            using(var writer = new StreamWriter(new FileStream("", FileMode.OpenOrCreate)))
            {
                JSON.Serialize(site, writer);
            }
        }

        private void MakeDirectoryIfNotExists(string path)
        {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}