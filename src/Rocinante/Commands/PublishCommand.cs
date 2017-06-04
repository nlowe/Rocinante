using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Html;
using NLog;
using RazorLight;
using Rocinante.Types;
using Rocinante.Types.Extensions;
using Rocinante.Types.ViewModels;

namespace Rocinante.Commands
{
    public class PublishCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name => "publish";

        public string Description => "Render the site to the output directory";

        public string Help => @"
Renders all posts using the site template to the output directory.
If the output directory exists, it will be deleted before the site
is published.

The publish directory is chosen from the site file or provided as the
only argument to the publish command.
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

            var publishDirectory = Path.Combine(root, args.Length == 1 ? args[0] : site.DefaultPublishLocation);

            if(Directory.Exists(publishDirectory))
            {
                Log.Warn("Removing old publish directory {0}", publishDirectory);
                Directory.Delete(publishDirectory, recursive: true);
            }

            publishDirectory.MakeDirectoryIfNotExists();
            var pageRenderer = EngineFactory.CreatePhysical(Path.Combine(root, "_theme"));

            Log.Info("Beginning publish for {0} at {1}", site.Name, publishDirectory);
            foreach(var p in ctx.RenderedPosts(site))
            {
                var effectivePath = site.PublishPathFor(p, args.Length == 0 ? null : args[0]);
                Path.GetDirectoryName(effectivePath).MakeDirectoryIfNotExists();
                Log.Debug("Rendering {0} to {1}", p.Title, effectivePath);

                var model = new PostViewModel 
                {
                    Site = site,
                    PublishContext = ctx,
                    Post = p
                };
               
                var html = pageRenderer.Parse("Post.cshtml", model);

                Log.Trace("Writing post {0}", p.Title);
                File.WriteAllText(effectivePath, html);
            }

            Log.Debug("Writing Index Page");
            var indexModel = new RocinanteViewModel
            {
                Site = site,
                PublishContext = ctx
            };

            var index = pageRenderer.Parse("Index.cshtml", indexModel);
            File.WriteAllText(Path.Combine(publishDirectory, "index.html"), index);
        }
    }
}