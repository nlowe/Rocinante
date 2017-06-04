using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using Microsoft.AspNetCore.Html;
using NLog;
using Rocinante.Types;

namespace Rocinante
{
    public class PublishContext : IPublishContext
    {
        private readonly Regex H1Regex = new Regex(@"<h1.*<\/h1>", RegexOptions.Compiled | RegexOptions.Singleline);

        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly List<RenderedPost> PostCache = new List<RenderedPost>();

        private readonly List<IContentEngine> contentEngines = new List<IContentEngine>();
        public IEnumerable<IContentEngine> ContentEngines => contentEngines;

        private readonly List<IThemeResolver> themeResolvers = new List<IThemeResolver>();
        public IEnumerable<IThemeResolver> ThemeResolvers => themeResolvers;

        private readonly List<ICommand> commands = new List<ICommand>();
        public IEnumerable<ICommand> Commands => commands;

        private readonly IContainer dependencyResolver;

        public PublishContext(IContainer dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public IPublishContext UseContentEngine<T>() where T : IContentEngine
        {
            contentEngines.Add(dependencyResolver.Resolve<T>());
            Log.Trace("Registered Content Engine {0}", typeof(T).FullName);
            return this;
        }

        public IPublishContext UseThemeResolver<T>() where T : IThemeResolver
        {
            themeResolvers.Add(dependencyResolver.Resolve<T>());
            Log.Trace("Registered Theme Resolver {0}", typeof(T).FullName);
            return this;
        }

        public IPublishContext UseCommand<T>() where T : ICommand
        {
            commands.Add(dependencyResolver.Resolve<T>());
            Log.Trace("Registered Command {0}", typeof(T).FullName);
            return this;
        }

        public IEnumerable<RenderedPost> RenderedPosts(Site site)
        {
            if(PostCache.Count == 0)
            {
                Log.Debug("Creating Post Cache");

                foreach(var post in site.Posts())
                {
                    Log.Debug("Converting {0} from {1} to html", post.Title, post.Markup);
                    var engine = ContentEngines.FirstOrDefault(e => e.CanRender(post));
                    if(engine == null)
                    {
                        Log.Fatal("Unable to find a content engine for markup {0}. Are you missing a plugin?", post.Markup);
                        Environment.Exit(ExitCodes.NO_CONTENT_ENGINE);
                    }

                    var renderedPost = engine.Render(post);
                    if(site.TrimFirstH1)
                    {
                        if(renderedPost.PreviewContent != null)
                        {
                            Log.Trace("Stripping first H1 from preview");
                            
                            var preview = renderedPost.PreviewContent.Value;
                            var previewMatch = H1Regex.Match(preview);
                            if(previewMatch.Success)
                            {
                                renderedPost.PreviewContent = new HtmlString(preview.Substring(0, previewMatch.Index) + preview.Substring(previewMatch.Index + previewMatch.Length));
                            }
                        }

                        Log.Trace("Stripping first H1 from post body");
                        var body = renderedPost.RenderedContent.Value;
                        var match = H1Regex.Match(body);
                        if(match.Success)
                        {
                            renderedPost.RenderedContent = new HtmlString(body.Substring(0, match.Index) + body.Substring(match.Index + match.Length));
                        }
                    }

                    PostCache.Add(renderedPost);
                }
            }
            
            return PostCache;
        }
    }
}