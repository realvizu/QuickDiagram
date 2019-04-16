using System.Text.RegularExpressions;

namespace Codartis.Util
{
    public static class StringExtensions
    {
        public static string RemovePrefix(this string s, string prefix)
        {
            return s.StartsWith(prefix)
                ? s.Substring(prefix.Length)
                : s;
        }

        public static string ToSingleWhitespaces(this string s)
        {
            return s == null ? null : Regex.Replace(s, @"\s+", " ");
        }
    }
}
