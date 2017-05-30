using System;
using System.IO;
using System.Linq;
using Jil;
using NLog;
using Rocinante.Types;

namespace Rocinante.Commands
{
    public class RestoreCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name => "restore";

        public string Description => "restore plugins and themes for the current site";

        public string Help => @"
Restores the theme for the current site and downloads any plugins

Options:
* --force: re-download all plugins and theme
";

        public void Execute(string[] args, IPublishContext ctx)
        {
            var root = Directory.GetCurrentDirectory();
            var siteFile = Path.Combine(root, "site.json");

            Site site = null;
            try
            {
                site = Site.LoadFrom(siteFile);
            }
            catch(NoSuchSiteException)
            {
                Log.Fatal("There doesn't seem to be a rocinante site here. Did you run 'roci new' ?");
                Environment.Exit(ExitCodes.NO_SITE);
            }

            if(site == null)
            {
                Log.Fatal("Failed to load site from {0}", siteFile);
                Environment.Exit(ExitCodes.THE_WORLD_HAS_ENDED);
            }

            var force = args.Length == 2 && args[1] == "--force";

            RestorePlugins(site, ctx, force);
            var resolver = ctx.ThemeResolvers.FirstOrDefault(r => r.CanResolveFor(site));

            if(resolver == null)
            {
                Log.Fatal("Could not find a theme resolver for them {0}", site.Theme);
                Environment.Exit(ExitCodes.NO_THEME_RESOLVER);
            }

            Log.Trace("Found theme resolver {0}", resolver.GetType().Name);

            var themeDir = Path.Combine(root, "_theme");
            if(!Directory.Exists(themeDir) || force)
            {
                if(Directory.Exists(themeDir))
                {
                    Log.Debug("Removing existing theme");
                    Directory.Delete(themeDir, recursive: true);
                }

                Log.Info("Downloading theme {0}", site.Theme);

                Directory.CreateDirectory(themeDir);
                resolver.ResolveTheme(site, themeDir);
            }
        }

        private void RestorePlugins(Site site, IPublishContext ctx, bool force)
        {
            Log.Debug("Restoring plugins for site '{0}'", site.Name);

            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "_plugins");
            if(!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

            foreach(var plugin in site.Plugins)
            {
                Log.Trace("Fetching {0}@{1}", plugin.Key, plugin.Value);
                // TODO: Only download plugin and dependencies if missing or forced
                // TODO: Load plugin
            }
        }
    }
}