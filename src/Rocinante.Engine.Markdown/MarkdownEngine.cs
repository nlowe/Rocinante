using System;
using Rocinante.Types;
using Markdig;

namespace Rocinante.Engine.Markdown
{
    public class MarkdownEngine : IContentEngine, IRocinantePlugin
    {
        private readonly MarkdownPipeline pipeline;

        public const string IDENTIFIER = "rocinante.engine.markdown";
        public string Identifier => IDENTIFIER;

        public string Name => "Markdown Content Engine";

        public string Description => "A content engine for rendering markdown posts";

        public MarkdownEngine()
        {
            pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        public void OnLoad(IPublishContext ctx)
        {
            ctx.UseContentEngine<MarkdownEngine>();
        }

        public string Render(Post post) => Markdig.Markdown.ToHtml(post.Content, pipeline);
    }
}
