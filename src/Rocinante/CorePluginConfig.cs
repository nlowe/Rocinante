using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyModel;
using NLog;
using Rocinante.Commands;
using Rocinante.Types;

namespace Rocinante
{
    internal static class CorePluginConfig
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static IPublishContext ConfigurePlugins(IContainer container)
        {
            Log.Debug("Registering Commands And Plugins");
            var ctx = new PublishContext(container);
            new DefaultCommandPlugin().OnLoad(ctx);
            ctx.UseThemeResolver<LocalThemeResolver>();

            Log.Trace("Inspecting Runtime Libraries");
            foreach(var library in DependencyResolver.GetCandidateLibraries(DependencyContext.Default))
            {
                Log.Trace("Inspecing library {0}", library.Name);
                foreach(var assembly in library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths))
                {
                    Log.Trace("Inspecting assembly {0}", assembly);
                    var loadedAssembly = Assembly.Load(new AssemblyName(assembly.Replace(".dll", "")));

                    foreach(var plugin in loadedAssembly.ExportedTypes.Where(t => typeof(IRocinantePlugin).IsAssignableFrom(t) && t.GetTypeInfo().IsClass))
                    {
                        Log.Debug("Loading Plugin {0}", plugin.FullName);
                        ((IRocinantePlugin)container.Resolve(plugin)).OnLoad(ctx);
                    }
                }
            }

            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "_plugins");
            if(Directory.Exists(pluginPath))
            {
                Log.Trace("Loading downloaded plugins from {0}", pluginPath);
            }

            return ctx;
        }
    }
}