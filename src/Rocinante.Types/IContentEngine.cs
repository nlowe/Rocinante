namespace Rocinante.Types
{
    public interface IContentEngine
    {
        /// <summary>
        /// The identifier for the content engine
        /// </summary>
        string Identifier { get;}

        /// <summary>
        /// Render the specified post to html
        /// </summary>
        /// <param name="post">The post to render</param>
        /// <returns>The html representation of the post</returns>
        string Render(Post post);
    }
}