using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyModel;
using NLog;
using NLog.Config;
using NLog.Targets;
using Rocinante.Commands;
using Rocinante.Types;

namespace Rocinante
{
    class Program : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly CommandMap CommandMap = new CommandMap();

        private static IContainer Container;

        static void Main(string[] args)
        {
            ConfigureLogging(LogLevel.Trace);
            var builder = ConfigureDependencies();
            Container = ConfigureCommands(builder);

            if(args.Length == 0)
            {
                CommandMap["help"].Execute(new string[]{});
                return;
            }

            ICommand cmd;
            if(!CommandMap.TryGetValue(args[0], out cmd))
            {
                Log.Fatal("Unknown command {0}", args[0]);
                Environment.Exit(-1);
            }

            Log.Trace("Executing command {0}", cmd.Name);

            var remainingArgs = new string[args.Length - 1];
            Array.Copy(args, 1, remainingArgs, 0, args.Length - 1);
            cmd.Execute(remainingArgs);
        }

        private static IContainer ConfigureCommands(ContainerBuilder builder)
        {
            Log.Debug("Registering Commands");
            var commands = new List<Type>();

            Log.Trace("Registering Core Commands");

            commands.AddRange(typeof(HelpCommand).GetTypeInfo().Assembly.GetExportedTypes().Where(t => typeof(ICommand).IsAssignableFrom(t) && t.GetTypeInfo().IsClass));

            Log.Trace("Inspecting Runtime Libraries");
            foreach(var assembly in DependencyContext.Default.RuntimeLibraries.SelectMany(lib => lib.Assemblies))
            {
                Log.Trace("Inspecting Assembly {0}", assembly.Name);
                var loadedAssembly = Assembly.Load(assembly.Name);
                
                commands.AddRange(loadedAssembly.GetExportedTypes().Where((t => typeof(ICommand).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)));
            }

            Log.Trace("Inspecting Compile Libraries");
            foreach(var assembly in DependencyContext.Default.CompileLibraries.SelectMany(lib => lib.Assemblies))
            {
                Log.Trace("Inspecting Assembly {0}", assembly);
                var loadedAssembly = Assembly.Load(new AssemblyName(assembly));
                
                commands.AddRange(loadedAssembly.GetExportedTypes().Where((t => typeof(ICommand).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)));
            }

            if(commands.Count == 0) Log.Warn("No commands found. Is Rocinante installed correctly?");

            foreach(var type in commands)
            {
                builder.RegisterType(type);
            }

            var container = builder.Build();

            foreach(var cmd in commands)
            {
                var instance = (ICommand) container.Resolve(cmd);

                Log.Trace("Registered {0} as {1}", cmd.Name, instance.Name);
                CommandMap.Add(instance.Name.ToLower(), instance);
            }

            return container;
        }

        private static ContainerBuilder ConfigureDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(CommandMap).ExternallyOwned();

            return builder;
        }

        private static void ConfigureLogging(LogLevel verbosity)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = @"[${pad:padding=5:inner=${level:uppercase=true}}] [${logger}] ${message}";

            var rule1 = new LoggingRule("*", verbosity, consoleTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(Container != null) Container.Dispose();
                }

                Container = null;

                disposedValue = true;
            }
        }

        ~Program() {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}