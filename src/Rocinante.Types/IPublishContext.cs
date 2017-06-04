using System;
using System.Collections.Generic;

namespace Rocinante.Types
{
    public interface IPublishContext
    {
        /// <summary>
        /// All content engines registered so far
        /// </summary>
        IEnumerable<IContentEngine> ContentEngines { get; }

        /// <summary>
        /// Register the specified type as a content engine
        /// </summary>
        IPublishContext UseContentEngine<T>() where T : IContentEngine;

        /// <summary>
        /// All theme resolvers registered so far
        /// </summary>
        IEnumerable<IThemeResolver> ThemeResolvers { get; }

        /// <summary>
        /// Register the specified type as a theme resolver
        /// </summary>
        IPublishContext UseThemeResolver<T>() where T : IThemeResolver;

        /// <summary>
        /// All commands registered so far
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Register the specified type as a command
        /// </summary>
        IPublishContext UseCommand<T>() where T : ICommand;

        /// <summary>
        /// Process and return all posts for the site
        /// </summary>
        IEnumerable<RenderedPost> RenderedPosts(Site site);
    }
}