using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
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
        private static readonly Logger Log = LogManager.GetLogger("roci");

        private static IContainer Container;

        static void Main(string[] args)
        {
            var logLevel = LogLevel.Info;
            var cmdIndex = 0;

            if(args.Length > 0)
            {
                foreach(var arg in args)
                {
                    if((arg.StartsWith("-v") || arg.StartsWith("--verbose")) && arg.Contains('='))
                    {
                        var parts = arg.Split('=');
                        if(parts.Length != 2)
                        {
                            Log.Fatal("Invalid syntax for verbosity parameter");
                            Environment.Exit(ExitCodes.SYNTAX);
                        }

                        logLevel = LogLevel.FromString(parts[1]);
                        cmdIndex++;
                    }
                }
            }

            ConfigureLogging(logLevel);
            Container = ConfigureDependencies();
            var ctx = ConfigurePlugins(Container);

            if(args.Length == 0)
            {
                new HelpCommand().Execute(new string[]{}, ctx);
                Environment.Exit(ExitCodes.SUCCESS);
            }

            Log.Trace("Looking for command {0}", args[cmdIndex]);
            var cmd = ctx.Commands.FirstOrDefault(c => c.Name.Equals(args[cmdIndex], StringComparison.CurrentCultureIgnoreCase));
            if(cmd == null)
            {
                Log.Fatal("Unknown command {0}", args[cmdIndex]);
                Environment.Exit(ExitCodes.UNKNOWN_COMMAND);
            }

            Log.Trace("Executing command {0}", cmd.Name);

            var remainingArgs = new string[args.Length - 1 - cmdIndex];
            Array.Copy(args, cmdIndex + 1, remainingArgs, 0, args.Length - 1 - cmdIndex);
            cmd.Execute(remainingArgs, ctx);
        }

        private static IPublishContext ConfigurePlugins(IContainer container)
        {
            Log.Debug("Registering Commands And Plugins");
            var ctx = new PublishContext(container);

            Log.Trace("Registering Core Commands");            
            new DefaultCommandPlugin().OnLoad(ctx);

            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "_plugins");
            if(Directory.Exists(pluginPath))
            {
                Log.Trace("Loading plugins from {0}", pluginPath);
            }

            return ctx;
        }

        private static IContainer ConfigureDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            return builder.Build();
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