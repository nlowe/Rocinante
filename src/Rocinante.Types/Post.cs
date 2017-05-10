using System;
using System.Collections.Generic;

namespace Rocinante.Types
{
    public class Post
    {
        /// <summary>
        /// The title for the post
        /// </summary>
        public string Title { get; set; } = "Untitled Post";

        /// <summary>
        /// The date the post was published
        /// </summary>
        public DateTime PublishedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// The author of the post. If null or empty, the site-level author is rendered
        /// </summary>
        public string Author { get; set; } = "";

        /// <summary>
        /// Any tags for this post
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// The raw content of the post, transformed by the markup engine specified in <see cref="Markup" />
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// The markup engine to use
        /// </summary>
        public string Markup { get; set; } = "rocinante.engine.markdown";
    }
}