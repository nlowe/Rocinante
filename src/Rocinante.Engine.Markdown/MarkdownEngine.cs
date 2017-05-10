using System;
using Rocinante.Types;

namespace Rocinante.Engine.Markdown
{
    public class MarkdownEngine : IContentEngine
    {
        public const string IDENTIFIER = "rocinante.engine.markdown";
        public string Identifier { get; } = IDENTIFIER;

        public string Render(Post post)
        {
            return "<h1>Not Implemented</h1>";
        }
    }
}
