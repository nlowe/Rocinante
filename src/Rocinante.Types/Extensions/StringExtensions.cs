namespace Rocinante.Types.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static string OrNull(this string str) => str.IsNullOrEmpty() ? null : str;

        public static string OrBlank(this string str) => str.IsNullOrWhiteSpace() ? null : str;

        public static string AppendIfMissing(this string str, string suffix) => str.EndsWith(suffix) ? str : str + suffix;
    }
}