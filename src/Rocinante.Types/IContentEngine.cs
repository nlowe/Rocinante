namespace Rocinante.Types
{
    public interface IContentEngine
    {
        /// <summary>
        /// The extension to use for new posts and drafts
        /// </summary>
        string PostExtension { get; }

        /// <summary>
        /// Determines if the content engine can render the specified post
        /// </summary>
        /// <param name="post">The post to check</param>
        /// <returns>True iff the post can be rendered by this content engine</returns>
        bool CanRender(Post post);

        /// <summary>
        /// Gets the template source for the specified post (including frontmatter)
        /// </summary>
        /// <param name="post">The post to generate a template for</param>
        /// <returns>The content of a template for the specified post</returns>
        string GetTemplateFor(Post post);

        /// <summary>
        /// Render the specified post to html
        /// </summary>
        /// <param name="post">The post to render</param>
        /// <returns>The processed post</returns>
        RenderedPost Render(Post post);
    }
}