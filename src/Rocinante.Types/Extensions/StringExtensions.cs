namespace Rocinante.Types.Extensions
{
    public static class StringExtensions
    {
        public static string OrNull(this string str) => string.IsNullOrEmpty(str) ? null : str;

        public static string AppendIfMissing(this string str, string suffix) => str.EndsWith(suffix) ? str : str + suffix;
    }
}