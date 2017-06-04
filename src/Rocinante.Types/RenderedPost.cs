using Microsoft.AspNetCore.Html;

namespace Rocinante.Types
{
    public class RenderedPost : Post
    {
        public HtmlString PreviewContent { get; set; } = null;
        public HtmlString RenderedContent { get; set; }

        public RenderedPost(Post post)
        {
            this.Title = post.Title;
            this.UrlTitle = post.UrlTitle;
            this.PublishedOn = post.PublishedOn;
            this.Author = post.Author;
            this.Tags = post.Tags;
            this.Content = post.Content;
            this.Markup = post.Markup;
        }
    }
}