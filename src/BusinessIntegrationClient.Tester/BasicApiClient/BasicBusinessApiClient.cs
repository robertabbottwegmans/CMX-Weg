using System;
using System.Globalization;
using System.IO;
using System.Net;
using log4net;
using Newtonsoft.Json;

namespace BusinessIntegrationClient.Tester.BasicApiClient
{
    public class BasicBusinessApiClient
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string TimestampHeaderName = "Timestamp";

        private readonly ILog _logger  = LogManager.GetLogger("API");
        private readonly RqlApiConfiguration _apiConfig;
        private AuthenticateResponse _authenticateResponse;
        private DateTime _authenticationTicketIssued;

        public BasicBusinessApiClient(RqlApiConfiguration apiConfig)
        {
            if (apiConfig == null) throw new ArgumentNullException(nameof(apiConfig));
            if (string.IsNullOrEmpty(apiConfig.Site))
                throw new ArgumentException("SiteName cannot be null or empty", nameof(apiConfig));
            if (string.IsNullOrEmpty(apiConfig.UserName))
                throw new ArgumentException("UserName cannot be null or empty", nameof(apiConfig));
            if (string.IsNullOrEmpty(apiConfig.Password))
                throw new ArgumentException("Password cannot be null or empty", nameof(apiConfig));

            _apiConfig = apiConfig;

            EnableTls();
        }

        private void EnableTls()
        {
            //do this before making API requests
            System.Net.ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public TResponse GetJson<TResponse>(string relativeUrl, object data = null) where TResponse : class
        {
            return SendBizApiRequest<TResponse>("GET", relativeUrl, data);
        }

        public TResponse PutJson<TResponse>(string relativeUrl, object data) where TResponse : class
        {
            return SendBizApiRequest<TResponse>("PUT", relativeUrl, data);
        }

        public TResponse PostJson<TResponse>(string relativeUrl, object data) where TResponse : class
        {
            return SendBizApiRequest<TResponse>("POST", relativeUrl, data);
        }

        public void Delete(string relativeUrl)
        {
            SendBizApiRequest<string>("DELETE", relativeUrl, null);
        }

        private TResponse SendBizApiRequest<TResponse>(string method, string relativeUrl, object data)
            where TResponse : class
        {
            ReauthenticateIfNearingExpiration();

            var bizApiUrl = MakeBizApiUrl(relativeUrl);

            return SendJsonRequestInternal<TResponse>(_authenticateResponse, method,
                bizApiUrl, ToJson(data));
        }

        private TResponse SendJsonRequestInternal<TResponse>(AuthenticateResponse authorization, string method, string url, string body) where TResponse : class
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.Accept = ContentType.Json;

                if (authorization != null)
                {
                    //these headers are required for requests needing authentication.
                    request.Headers.Add(AuthorizationHeaderName, authorization.Ticket);
                    //include the client's current timestamp so the server
                    //can check it agains the authorization ticket
                    request.Headers.Add(TimestampHeaderName, ToUtcRfc1123(DateTime.Now));
                }

                _logger.DebugFormat("Sending {0} request: url: {1}, Body: {2}", method, url, body);

                if (!string.IsNullOrEmpty(body))
                {
                    request.ContentType = "application/json";
                    request.ContentLength = body.Length;

                    using (var sw = new StreamWriter(request.GetRequestStream()))
                    {
                        sw.Write(body);
                    }
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    _logger.DebugFormat("Response: {0} - {1}", (int) response.StatusCode, response.StatusDescription);
                    
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            var responseJson = sr.ReadToEnd();

                            _logger.Debug(responseJson);

                            var result = typeof(TResponse) == typeof(string)
                                ? responseJson as TResponse
                                : JsonConvert.DeserializeObject<TResponse>(responseJson);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("Exception sending {0} web request to URL: {1}.{2}", method, url, ex);
                throw;
            }
        }

        public void Authenticate()
        {
            var authenticateUrl = _apiConfig.GetAuthenticationUrl().ToString();

            var body = ToJson(new { _apiConfig.UserName, _apiConfig.Password });

            _authenticateResponse = SendJsonRequestInternal<AuthenticateResponse>(
                null, "POST", authenticateUrl, body);

            _authenticationTicketIssued = FromRfc1123(_authenticateResponse.TicketExpires).ToLocalTime();
        }

        private static string ToJson(object obj)
        {
            if (obj == null) return null;
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        private static DateTime FromRfc1123(string rfc1123Date)
        {
            //RFC1123 date example: Tue, 11 Sep 2018 00:15:05 GMT
            return DateTime.ParseExact(rfc1123Date, CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture);
        }

        private string ToUtcRfc1123(DateTime value)
        {
            return value.ToUniversalTime().ToString(CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Re-authenticates the api client connection if the authentication ticket is nearing expiration.
        /// </summary>
        /// <remarks>
        ///     This method is provided to prevent long running processes from failing because the Authentication Ticket has expired
        /// </remarks>
        private void ReauthenticateIfNearingExpiration()
        {
            Func<bool> shouldAuthenticate = () =>
                _authenticateResponse == null ||
                (DateTime.Now - _authenticationTicketIssued).TotalMinutes >= 40;

            if (shouldAuthenticate())
            {
                lock (this)
                {
                    if (!shouldAuthenticate()) return;

                    try
                    {
                        Authenticate();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Unable to re-authenticate the API connection. Exception:", ex);
                        throw;
                    }
                }
            }
        }

        private string MakeBizApiUrl(string relativeUrl)
        {
            var url = new Uri(_apiConfig.GetBusinessApiBaseUri(), relativeUrl);
            return url.ToString();
        }

        #region Nested Types
        /// <summary>
        /// Represents the response from the /api/Authenticate API
        /// </summary>
        private class AuthenticateResponse
        {
            /// <summary>
            ///     The authentication ticket. Valid for 1 hour.
            /// </summary>
            public string Ticket { get; set; }

            /// <summary>
            ///     The date the ticket was issued (is named wrong).
            /// </summary>
            /// <remarks>
            ///     Is in RFC 1123 format.
            /// </remarks>
            public string TicketExpires { get; set; }
        }

        #endregion
    }
}