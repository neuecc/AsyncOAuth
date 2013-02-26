using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncOAuth
{
    internal static class Utility
    {
        static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long ToUnixTime(this DateTime target)
        {
            return (long)(target - unixEpoch).TotalSeconds;
        }

        /// <summary>Escape RFC3986 String</summary>
        public static string UrlEncode(this string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape)
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29");
        }


        public static string UrlDecode(this string stringToUnescape)
        {
            return Uri.UnescapeDataString(stringToUnescape)
                .Replace("%21", "!")
                .Replace("%2A", "*")
                .Replace("%27", "'")
                .Replace("%28", "(")
                .Replace("%29", ")");
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string query)
        {
            var queryParams = query.TrimStart('?').Split('&')
               .Where(x => x != "")
               .Select(x =>
               {
                   var xs = x.Split('=');
                   return new KeyValuePair<string, string>(xs[0], xs[1]);
               });

            return queryParams;
        }

        public static string Wrap(this string input, string wrapper)
        {
            return wrapper + input + wrapper;
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}