using System;
using System.IO;
using Rocinante.Types;
using Jil;
using NLog;
using Rocinante.Types.Extensions;
using System.Linq;

namespace Rocinante.Commands
{
    public class NewCommand : ICommand
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Name => "new";

        public string Description => "Create a new site, post, or draft";

        public string Help => @"
syntax: roci new <site | post <name> [markup] | draft <name> [markup]>

site:
    Initializes the current directory with the default site template:
     * A folder for posts and drafts
     * site.json: The configuration for the site

post:
    Creates a new post in the posts folder with the specified name

draft:
    Creates a new draft in the drafts folder with the specified name

";

        public void Execute(string[] args, IPublishContext ctx)
        {

            var root = Directory.GetCurrentDirectory();
            var siteFile = Path.Combine(root, "site.json");

            if (args.Length == 0 || args[0].Equals("site", StringComparison.OrdinalIgnoreCase))
            {
                NewSite(root, siteFile);
                return;
            }
            else if (args.Length < 2)
            {
                Log.Fatal("Invalid syntax. Expecting at least one of 'new [site|post|draft]'");
                Environment.Exit(ExitCodes.SYNTAX);
            }

            NewPostOrDraft(args, ctx, root, siteFile);
        }

        private static void NewPostOrDraft(string[] args, IPublishContext ctx, string root, string siteFile)
        {
            if (!File.Exists(siteFile))
            {
                Log.Fatal("There doesn't seem to be a rocinante site here. Did you run 'roci new site' ?");
                Environment.Exit(ExitCodes.NO_SITE);
            }

            var site = JSON.Deserialize<Site>(File.ReadAllText(siteFile));

            string subPath = null;
            switch (args[0].ToLower())
            {
                case "post": subPath = site.PostSource; break;
                case "draft": subPath = site.DraftSource; break;
            }

            if (subPath == null)
            {
                Log.Fatal("Unexpected argument: {0}. Was expecting one of {site, post, draft}", args[0]);
                Environment.Exit(ExitCodes.SYNTAX);
            }

            var markup = site.DefaultMarkup;
            if (args.Length > 2)
            {
                markup = args[2];
            }

            var post = new Post
            {
                Title = args[1],
                Markup = markup
            };

            var engine = ctx.ContentEngines.FirstOrDefault(e => e.CanRender(post));

            if (engine == null)
            {
                Log.Fatal("No content engine can render a post with markup {0}. Are you missing a plugin?", markup);
                Environment.Exit(ExitCodes.NO_CONTENT_ENGINE);
            }

            var template = engine.GetTemplateFor(post);

            var postPath = Path.Combine(root, subPath, post.PublishedOn.ToString(site.PostDateUrlFormat), $"{post.Title}.{engine.PostExtension}");

            if (File.Exists(postPath))
            {
                Log.Fatal("A post or draft already exists at {0}. Delete it and run the command again if you want to recreate it", postPath);
                Environment.Exit(ExitCodes.POST_ALREADY_EXISTS);
            }

            Path.GetDirectoryName(postPath).MakeDirectoryIfNotExists();

            File.WriteAllText(postPath, template);

            Log.Info("New post created at {0}", postPath);
        }

        private void NewSite(string root, string siteFile)
        {
            if(File.Exists(siteFile))
            {
                Log.Fatal("There appears to already be a site here. Delete {0} if you want to recreate the site", siteFile);
                Environment.Exit(ExitCodes.SITE_ALREADY_EXISTS);
            }

            var site = new Site();
            var posts = Path.Combine(root, site.PostSource);
            var drafts = Path.Combine(root, site.DraftSource);

            posts.MakeDirectoryIfNotExists();
            drafts.MakeDirectoryIfNotExists();

            Log.Info("Creating default site configuration");
            using(var writer = new StreamWriter(new FileStream(siteFile, FileMode.OpenOrCreate)))
            {
                JSON.Serialize(site, writer, Options.ISO8601PrettyPrintExcludeNulls);
            }

            Log.Info("Site created at {0}", root);
        }
    }
}