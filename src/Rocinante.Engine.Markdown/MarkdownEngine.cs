using System;
using Rocinante.Types;
using Markdig;
using System.Linq;
using Rocinante.Types.Extensions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;

namespace Rocinante.Engine.Markdown
{
    public class MarkdownEngine : IContentEngine, IRocinantePlugin
    {
        private readonly Regex PreviewRegex = new Regex(@"^<!--(?:\s+)?more(?:\s+)?-->$", RegexOptions.Compiled | RegexOptions.Multiline);

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

        public RenderedPost Render(Post post)
        {
            var more = PreviewRegex.Match(post.Content);
            string renderedPreview = null;
            if(more.Success)
            {
                renderedPreview = Markdig.Markdown.ToHtml(post.Content.Substring(0, more.Index), pipeline);
            }

            return new RenderedPost(post)
            {
                PreviewContent = renderedPreview == null ? null : new HtmlString(renderedPreview),
                RenderedContent = new HtmlString(Markdig.Markdown.ToHtml(post.Content, pipeline))
            };
        }

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
