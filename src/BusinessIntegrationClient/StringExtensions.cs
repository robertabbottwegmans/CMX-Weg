using System.Web;

namespace BusinessIntegrationClient
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        public static bool IsUrlEncoded(this string value)
        {
            return value != HttpUtility.UrlDecode(value);
        }
    }
}