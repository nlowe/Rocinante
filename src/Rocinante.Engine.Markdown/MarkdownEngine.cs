using System;
using Rocinante.Types;
using Markdig;
using System.Linq;
using Rocinante.Types.Extensions;

namespace Rocinante.Engine.Markdown
{
    public class MarkdownEngine : IContentEngine, IRocinantePlugin
    {
        private readonly MarkdownPipeline pipeline;

        public string Name => "Markdown Content Engine";

        public string Description => "A content engine for rendering markdown posts";

        public string PostExtension => "md";

        public MarkdownEngine()
        {
            pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        public void OnLoad(IPublishContext ctx)
        {
            ctx.UseContentEngine<MarkdownEngine>();
        }

        public string Render(Post post) => Markdig.Markdown.ToHtml(post.Content, pipeline);

        public bool CanRender(Post post) => new []{"md", "markdown"}.Contains(post.Markup.ToLower());

        public string GetTemplateFor(Post post) => $@"
---
markup: md
publishedOn: {post.PublishedOn}
title: {post.Title}{(!post.Author.IsNullOrWhiteSpace() ? $"\nauthor: {post.Author}" : "")}
---

# {post.Title}
New Draft
";
    }
}
