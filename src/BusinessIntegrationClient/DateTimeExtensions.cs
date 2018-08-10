using System;
using System.Globalization;

namespace BusinessIntegrationClient
{
    public static class DateTimeExtensions
    {
        public static DateTime FromRfc1123(this string s)
        {
            return DateTime.ParseExact(s, CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture);
        }

        public static string ToUtcRfc1123(this DateTime value)
        {
            return value.ToUniversalTime().ToString(CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture);
        }

    }
}