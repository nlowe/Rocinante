using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using Rocinante.Types.Extensions;

namespace Rocinante.Types
{
    public partial class Site
    {
        /// <summary>
        /// The path to the site on disk
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// The name of the site
        /// </summary>
        public string Name { get; set; } = "Untitled Site";

        /// <summary>
        /// The "tagline" or description of the site. Not rendered if null or empty
        /// </summary>
        public string Tagline { get; set; } = "";

        /// <summary>
        /// The author of the site. If the author of individual posts is not specified, this setting is used
        /// </summary>
        public string Author { get; set; } = "Anonymous";

        /// <summary>
        /// The theme for the site. If no protocol is specified, it is assumed the theme is a github repo that should be cloned.
        /// 
        /// Supported protocols: github
        /// </summary>
        public string Theme { get; set; } = "roci-gen/roci-theme-default";

        /// <summary>
        /// The root of the site
        /// </summary>
        public string Root { get; set; } = "/";

        /// <summary>
        /// The default markup to use for new posts and drafts
        /// </summary>
        public string DefaultMarkup { get; set; } = "md";

        /// <summary>
        /// The directory to look in for posts, relative to the site configuration file
        /// </summary>
        public string PostSource { get; set; } = "posts";

        /// <summary>
        /// The directory to look in for drafts, relative to the site configuration file
        /// </summary>
        /// <returns></returns>
        public string DraftSource { get; set; } = "_drafts";

        /// <summary>
        /// The directory to publish to if none is specified on the command line
        /// </summary>
        public string DefaultPublishLocation { get; set; } = "public";

        /// <summary>
        /// The date format for post URLs
        /// </summary>
        public string PostDateUrlFormat { get; set; } = "yyyy/MM";

        /// <summary>
        /// Any plugins to pull from nuget for use in site rendering
        /// </summary>
        public Dictionary<string, string> Plugins { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Resolve the author for the post
        /// </summary>
        /// <param name="post">The post to resolve the author for</param>
        /// <returns>The author from the post, or if null or empty, the site author</returns>
        public string AuthorFor(Post post) => post.Author.OrNull() ?? Author;

        /// <summary>
        /// The URL to the specified post (including the site root url)
        /// </summary>
        public string UrlFor(Post post)
        {
            return Root.AppendIfMissing("/") +
                post.PublishedOn.ToString(PostDateUrlFormat).AppendIfMissing("/") +
                UrlEncoder.Default.Encode(post.UrlTitle.OrBlank() ?? post.Title);
        }

        /// <summary>
        /// The path to publish the post to
        /// </summary>
        public string PublishPathFor(Post post, string publishDir = null)
        {
            return Path.Combine(Location, publishDir ?? DefaultPublishLocation, post.PublishedOn.ToString(PostDateUrlFormat), $"{post.Title}.html");
        }

        /// <summary>
        /// A collection of all posts in the site
        /// </summary>
        public IEnumerable<Post> Posts()
        {
            foreach(var file in Directory.GetFiles(Path.Combine(Location, PostSource), "*.*", SearchOption.AllDirectories))
            {
                Post post;
                if(PostParser.TryLoad(file, out post))
                {
                    yield return post;
                }
            }
        }

        /// <summary>
        /// A collection of all drafts in the site
        /// </summary>
        public IEnumerable<Post> Drafts()
        {
            foreach(var file in Directory.GetFiles(Path.Combine(Location, DraftSource), "*.*", SearchOption.AllDirectories))
            {
                Post post;
                if(PostParser.TryLoad(file, out post))
                {
                    yield return post;
                }
            }
        }
    }
}
