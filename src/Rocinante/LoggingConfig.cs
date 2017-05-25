using NLog;
using NLog.Config;
using NLog.Targets;

namespace Rocinante
{
    internal static class LoggingConfig
    {
        private const string LAYOUT = @"[${pad:padding=5:inner=${level:uppercase=true}}] [${logger}] ${message}";

        public static void ConfigureLogging(LogLevel verbosity)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = LAYOUT;

            var rule1 = new LoggingRule("*", verbosity, consoleTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }
    }
}