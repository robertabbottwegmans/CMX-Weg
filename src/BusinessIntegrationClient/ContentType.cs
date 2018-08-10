namespace BusinessIntegrationClient
{
    public static class ContentEncoding
    {
        public static readonly string Deflate = "deflate";
        public static readonly string Gzip = "gzip";
    }

    public static class ContentType
    {
        public static readonly string Atom = "application/atom+xml";
        public static readonly string Css = "text/css";
        public static readonly string Csv = "text/csv";
        public static readonly string Eot = "application/vnd.ms-fontobject";
        /// <summary>
        /// The Mime type for .xsl Excel before Excel2007
        /// </summary>
        public static readonly string ExcelXls = "application/vnd.ms-excel";
        /// <summary>
        /// The Mime type for .xlsx, Excel2007 or newer.
        /// </summary>
        public static readonly string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static readonly string Form = "application/x-www-form-urlencoded";
        public static readonly string Gif = "image/gif";
        public static readonly string Html = "text/html";
        public static readonly string Icon = "image/x-icon";
        public static readonly string Iqy = "text/x-ms-iqy";
        public static readonly string Javascript = "text/javascript";
        public static readonly string Jpeg = "image/jpeg";
        public static readonly string Json = "application/json";
        public static readonly string OctetStream = "application/octet-stream";
        public static readonly string OpenTypeFont = "application/font-sfnt";
        public static readonly string Pdf = "application/pdf";
        public static readonly string Png = "image/png";
        /// <summary>
        /// The content type fpr PNG files before it was adopted as a standard. This was in use in the platform for some time.
        /// </summary>
        public const string LegacyPng = "image/x-png";
        public static readonly string Rss = "application/rss+xml";
        public static readonly string Svg = "image/svg+xml";
        public static readonly string Swf = "application/x-shockwave-flash";
        public static readonly string Text = "text/plain";
        public static readonly string TrueTypeFont = "application/font-sfnt";
        public static readonly string Xml = "application/xml";
        public static readonly string Woff = "application/font-woff";

        public static string Get(string path)
        {
            string ext = System.IO.Path.GetExtension(path).ToLower();

            switch (ext)
            {
                case ".css":
                    return Css;
                case ".eot":
                    return Eot;
                case ".gif":
                    return Gif;
                case ".jpg":
                    return Jpeg;
                case ".htm":
                case ".html":
                    return Html;
                case ".js":
                    return Javascript;
                case ".otf":
                    return OpenTypeFont;
                case ".pdf":
                    return Pdf;
                case ".png":
                    return Png;
                case ".svg":
                    return Svg;
                case ".swf":
                    return Swf;
                case ".txt":
                    return Text;
                case ".xls":
                    return ExcelXls;
                case ".xlsx":
                    return Excel;
                case ".ttf":
                    return TrueTypeFont;
                case ".woff":
                    return Woff;
                case ".xml":
                    return Xml;
                default:
                    if (!string.IsNullOrEmpty(path) &&
                        path.ToLower().EndsWith(".css.map"))
                    {
                        return Json;
                    }
#if MONODROID
                    return Java.Net.URLConnection.GuessContentTypeFromName(path);
#elif MONOTOUCH
				throw new System.NotImplementedException();
#else
                    Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                    if (registryKey != null && registryKey.GetValue("Content Type") != null)
                        return registryKey.GetValue("Content Type").ToString();
                    break;
#endif
            }

            return null;
        }

        public static string GetExtension(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                    return ".jpg";
                case "image/png":
                    return ".png";
            }

            return null;
        }

    }
}
