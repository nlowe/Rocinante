using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Rocinante
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ConfigureLogging(LogLevel.Info);

            Log.Info("Hello, World!");
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
    }
}