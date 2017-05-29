namespace Rocinante
{
    public class ExitCodes
    {
        public const int THE_WORLD_HAS_ENDED = 3;
        public const int UNKNOWN_COMMAND = 2;
        public const int SYNTAX = 1;
        public const int SUCCESS = 0;
        public const int NO_SITE = -1;
        public const int NO_THEME_RESOLVER = -2;
        public const int SITE_ALREADY_EXISTS = -3;
        public const int NO_CONTENT_ENGINE = -4;
        public static int POST_ALREADY_EXISTS = -5;
    }
}