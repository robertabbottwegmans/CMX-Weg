using System;
using System.Configuration;

namespace BusinessIntegrationClient
{
    /// <summary>
    ///     A class representing connection configuration information
    /// </summary>
    public class RqlApiConfiguration
    {
        #region Constructors

        public RqlApiConfiguration()
        {
            UseSsl = true;
            RequestTimeout = TimeSpan.FromSeconds(30);
            ConnectionLeaseTimeout = TimeSpan.FromMinutes(5);
            Port = -1;
        }

        #endregion

        #region Properties
        /// <summary>
        ///     The Name of the Site, example: mysite.compliancemetrix.com
        /// </summary>
        public string Site { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        ///     Connection Lease Timeout value, Defaults to 5 minutes.
        ///     <see cref="http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html" />
        /// </summary>
        /// <remarks>
        ///     Instances of <see cref="System.Net.Http.HttpClient" /> will maintain a connection across requests, but if DNS
        ///     changes, it won't notice it as long as the connection is active. By forcing a connection least timeout, it'll
        ///     detect a DNS change.
        ///     <see cref="System.Net.Http.HttpClient" />
        ///     <see cref="http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html" />
        ///     See http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html
        /// </remarks>
        public TimeSpan ConnectionLeaseTimeout { get; set; }

        /// <summary>
        ///     The timeout associated with HTTP requests. Defaults to 30 seconds.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        ///     Specifies whether Ssl connection is required. Default to true.
        /// </summary>
        /// <remarks>
        ///     This is only false in Local Dev environments
        /// </remarks>
        public bool UseSsl { get; set; }

        /// <summary>
        ///     Specify a port if targetting a Platform instance running in a "LocalDev" environment.
        /// </summary>
        /// <remarks>
        ///     The default is -1, and doesn't need to be set unless specifically targeting a site in Local Dev. Local Dev is when
        ///     the Platform is hosted on the local development machine and uses self hosting on a port #.
        /// </remarks>
        public int Port { get; set; }

        /// <summary>
        ///     Optional UserAgent string to be added as the User-Agent HTTP Header on all requests.
        /// </summary>
        /// <remarks>
        ///     The Platform can evaluate this if needed, but it's ignored by default.
        ///     Basic Format is: {client name}/{version #}, eg:  My Client/1.0
        /// </remarks>
        public string UserAgent { get; set; }

        #endregion

        /// <summary>
        ///     Creates an instance of <see cref="RqlApiConfiguration" /> based on configuration cound in the app.config. See
        ///     remarks for required keys
        /// </summary>
        /// <returns></returns>
        public static RqlApiConfiguration FromAppConfig()
        {
            //This method assumes the following keys in app.config:
            //app.config:
            //<configuration>  
            //  <appSettings>                        
            //    <add key="Site" value="mysite.compliancemetrix.com"/>
            //    <add key="UserName" value="business.api.user"/>
            //    <add key="Password" value="{password goes here}"/>
            //    <add key="UseSsl" value="True"/>
            //    <add key="Port" value="-1"/>    
            //  </appSettings>
            //  <!-- snipped -->
            //</configuration>


            var site = ConfigurationManager.AppSettings["Site"];
            var userName = ConfigurationManager.AppSettings["UserName"];
            var password = ConfigurationManager.AppSettings["Password"];
            var useSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSsl"] ?? "True");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"] ?? "-1");
            var userAgent = ConfigurationManager.AppSettings["UserAgent"];

            return new RqlApiConfiguration
            {
                Site = site,
                UserName = userName,
                Password = password,
                UseSsl = useSsl,
                Port = port,
                UserAgent = userAgent
            };
        }

        /// <summary>
        ///     Gets full URL needed to POST an Authentication request to the Platform API.
        /// </summary>
        /// <returns>
        ///     The full URL to the authentication API endpoint.
        ///     Example: https://mysite.compliancemetrix.com/api/Authenticate
        /// </returns>
        public Uri GetAuthenticationUrl()
        {
            var uri = new UriBuilder(UseSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
                Site, Port, "api/Authenticate");

            return uri.Uri;
        }

        /// <summary>
        ///     Gets the Base URL needed for building request URLS for the Business API
        /// </summary>
        /// <returns>
        ///     Uri containing the URL to be used as the Base Business API URL for subsequent API calls.
        ///     Example: https://mysite.compliancemetrix.com/api/biz/
        /// </returns>
        /// <remarks>
        ///     Use this so that you can initialize a property such as <see cref="System.Net.Http.HttpClient.BaseAddress" /> or
        ///     <see cref="System.Net.WebClient.BaseAddress" /> and then use relative paths for subsequent API requests.
        /// </remarks>
        /// <example>
        ///     <code>
        ///      var apiConfig = RqlApiConfiguration.FromAppConfig();
        /// 
        ///      var httpClient = new System.Net.Http.HttpClient
        ///      {
        ///          BaseAddress = apiConfig.GetBusinessApiBaseUrl(),
        ///          RequestTimeout = apiConfig.RequestTimeout
        ///      };
        ///      //authentication steps not shown
        /// 
        ///      var getWidgetRelativePath = "Widgets/123";
        ///      var response = ImaginarySendGETRequestMethod(httpClient, getWidgetRelativePath);
        ///  
        ///  </code>
        /// </example>
        public Uri GetBusinessApiBaseUri()
        {
            var uri = new UriBuilder(UseSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
                Site, Port, "api/biz/");

            return uri.Uri;
        }
    }
}