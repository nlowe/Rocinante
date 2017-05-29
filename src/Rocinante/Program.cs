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
using Rocinante.Types.Extensions;

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

            LoggingConfig.ConfigureLogging(logLevel);
            Container = DependencyConfig.ConfigureDependencies();
            var ctx = CorePluginConfig.ConfigurePlugins(Container);

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

            try
            {
                cmd.Execute(remainingArgs, ctx);
            }
            catch(Exception ex)
            {
                Log.Fatal("{0} threw an exception: {1}\n{2}", cmd.Name, ex.AllInnerMessages(), ex.StackTrace);
                Environment.Exit(ExitCodes.THE_WORLD_HAS_ENDED);
            }
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