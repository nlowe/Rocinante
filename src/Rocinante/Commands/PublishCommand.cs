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
            foreach(var post in site.Posts())
            {
                var effectivePath = site.PublishPathFor(post, args.Length == 0 ? null : args[0]);
                Path.GetDirectoryName(effectivePath).MakeDirectoryIfNotExists();
                Log.Debug("Rendering {0} to {1}", post.Title, effectivePath);

                var engine = ctx.ContentEngines.FirstOrDefault(e => e.CanRender(post));
                if(engine == null)
                {
                    Log.Fatal("Unable to find a content engine for markup {0}. Are you missing a plugin?", post.Markup);
                    Environment.Exit(ExitCodes.NO_CONTENT_ENGINE);
                }

                Log.Trace("Rendering {0} with {1}", post.Title, engine.GetType().FullName);
                var src = engine.Render(post);
               
                var model = new PostViewModel 
                {
                    Site = site,
                    Post = post,
                    RenderedPost = new HtmlString(src)
                };
               
                var html = pageRenderer.Parse("Post.cshtml", model);

                Log.Trace("Writing post {0}", post.Title);
                File.WriteAllText(effectivePath, html);
            }

            Log.Debug("Writing Index Page");
            var indexModel = new IndexViewModel
            {
                Site = site
            };

            var index = pageRenderer.Parse("Index.cshtml", indexModel);
            File.WriteAllText(Path.Combine(publishDirectory, "index.html"), index);
        }
    }
}