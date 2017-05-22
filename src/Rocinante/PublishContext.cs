using System;
using System.Collections.Generic;
using Autofac;
using Rocinante.Types;

namespace Rocinante
{
    public class PublishContext : IPublishContext
    {
        private readonly List<IContentEngine> contentEngines = new List<IContentEngine>();
        public IEnumerable<IContentEngine> ContentEngines => contentEngines;

        private readonly List<IThemeResolver> themeResolvers = new List<IThemeResolver>();
        public IEnumerable<IThemeResolver> ThemeResolvers => themeResolvers;

        private readonly List<ICommand> commands = new List<ICommand>();
        public IEnumerable<ICommand> Commands => commands;

        private readonly IContainer dependencyResolver;

        public PublishContext(IContainer dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public IPublishContext UseContentEngine<T>() where T : IContentEngine
        {
            contentEngines.Add(dependencyResolver.Resolve<T>());
            return this;
        }

        public IPublishContext UseThemeResolver<T>() where T : IThemeResolver
        {
            themeResolvers.Add(dependencyResolver.Resolve<T>());
            return this;
        }

        public IPublishContext UseCommand<T>() where T : ICommand
        {
            commands.Add(dependencyResolver.Resolve<T>());
            return this;
        }
    }
}