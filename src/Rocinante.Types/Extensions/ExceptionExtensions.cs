using System;
using System.Text;

namespace Rocinante.Types.Extensions
{
    public static class ExceptionExtensions
    {
        public static string AllInnerMessages(this Exception ex, string seperator = "\n-----\n")
        {
            if(seperator.IsNullOrEmpty()) throw new ArgumentException("The seperator cannot be null or empty", nameof(seperator));

            var sb = new StringBuilder();

            var e = ex;
            while(e != null)
            {
                sb.Append(e.Message).Append(seperator);
                e = e.InnerException;
            }

            var result = sb.ToString();

            return result.Length > 0 ? result.Substring(0, result.Length - seperator.Length) : string.Empty;
        }
    }
}