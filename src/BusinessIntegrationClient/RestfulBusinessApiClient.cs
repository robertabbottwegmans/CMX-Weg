using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using log4net;

namespace BusinessIntegrationClient
{
    /// <summary>
    ///     Manages an <see cref="HttpClient" /> instance, making the necessary RESTful business API calls easier to invoke.
    /// </summary>
    public class RestfulBusinessApiClient : IDisposable
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string TimestampHeaderName = "Timestamp";

        private static readonly ILog Logger = LogManager.GetLogger("BusinessAPI");

        private readonly RqlApiConfiguration _apiConfig;
        private readonly HttpClient _httpClient;


        private DateTime _authenticationTicketIssued;
        
        /// <summary>
        /// Constructs a new instance of <see cref="RestfulBusinessApiClient"/>
        /// </summary>
        /// <param name="apiConfig"></param>
        public RestfulBusinessApiClient(RqlApiConfiguration apiConfig)
        {
            if (apiConfig == null) throw new ArgumentNullException("apiConfig");
            if (string.IsNullOrEmpty(apiConfig.Site))
                throw new ArgumentException("SiteName cannot be null or empty", "apiConfig");
            if (string.IsNullOrEmpty(apiConfig.UserName))
                throw new ArgumentException("UserName cannot be null or empty", "apiConfig");
            if (string.IsNullOrEmpty(apiConfig.Password))
                throw new ArgumentException("Password cannot be null or empty", "apiConfig");

            _apiConfig = apiConfig;

            _httpClient = CreateHttpClient(apiConfig);

            EnableTls();

            InitializeConnectionLeaseTimeout();
        }

        void IDisposable.Dispose()
        {
            _httpClient.Dispose();
        }

        private void EnableTls()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 |
                                                    SecurityProtocolType.Tls12;
        }

        private void InitializeConnectionLeaseTimeout()
        {
            var siteUri = _apiConfig.GetBusinessApiBaseUri().GetLeftPart(UriPartial.Authority);
            var timeout = _apiConfig.ConnectionLeaseTimeout.TotalMilliseconds;
            //See:
            //http://byterot.blogspot.com/2016/07/singleton-httpclient-dns.html

            var servicePoint = ServicePointManager.FindServicePoint(new Uri(siteUri));
            servicePoint.ConnectionLeaseTimeout = (int) timeout;
        }


        /// <summary>
        ///     Re-authenticates the api client connection if the authentication ticket is nearing expiration.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is provided to prevent long running processes from failing because the API client instance has
        ///         expired.
        ///     </para>
        ///     <para>
        ///         The ticket issued by the authentication request is only
        ///         valid for 1 hour.
        ///     </para>
        /// </remarks>
        public void ReauthenticateIfNearingExpiration()
        {

            Func<bool> shouldAuthenticate = () => (DateTime.Now - _authenticationTicketIssued).TotalMinutes >= 40;

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
                        Logger.Error("Unable to re-authenticate the API connection. Exception:", ex);
                        throw;
                    }
                }
            }
        }

        public void Authenticate()
        {
            var authenticateUrl = _apiConfig.GetAuthenticationUrl();

            var content = CreateJsonStringContent(new
            {
                _apiConfig.UserName,
                _apiConfig.Password
            }.ToJson());

            var response = _httpClient.PostAsync(authenticateUrl, content).Result;

            response.EnsureSuccessStatusCode();

            var authorization = response.Content.ReadAsStringAsync().Result
                .FromJson<AuthenticateResponse>();

            //note: despite the name, response.TicketExpires actually returns "DateTime.UtcNow" from the server side, 
            //making this the time the server issued the ticket, rather than when it expires. 
            //The server has logic internally to consider tickets expired.
            _authenticationTicketIssued = authorization.TicketExpires.FromRfc1123().ToLocalTime();

            _httpClient.DefaultRequestHeaders.Remove(AuthorizationHeaderName);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(AuthorizationHeaderName, authorization.Ticket);
        }


        private HttpClient CreateHttpClient(RqlApiConfiguration apiConfig)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = apiConfig.GetBusinessApiBaseUri(),
                Timeout = apiConfig.RequestTimeout
            };
            if (!string.IsNullOrEmpty(apiConfig.UserAgent))
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(apiConfig.UserAgent);

            httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(ContentType.Json)); //ACCEPT header

            return httpClient;
        }

        private string SendJsonRequest(HttpMethod method, string url, string requestBody)
        {
            var response = SendJsonRequest<string>(method, url, requestBody);

            return response;
        }

        private T SendJsonRequest<T>(HttpMethod method, string url,
            string requestBody)
            where T : class
        {
            ReauthenticateIfNearingExpiration();

            using (var content = CreateJsonStringContent(requestBody))
            using (var requestMessage = CreateJsonRequestMessage(method, url, content))
            {
                Logger.DebugFormat("Sending {0} request: url: {1}, Body: {2}", method, url, requestBody);

                if (method == HttpMethod.Delete)
                {
                    //When a DELETE returns 204 No Content,
                    //the next request was getting an exception:
                    //  "The server committed a protocol violation. Section=ResponseStatusLine"
                    //Setting the "KeepAlive = false" in DELETE requests fixes the problem.
                    //For performance, we're keeping connections alive, except for delete requests, which are the only ones expected to return 204 No Content.
                    //
                    requestMessage.Headers.ConnectionClose = true;
                }

                HttpResponseMessage response;
                try
                {
                    response = _httpClient.SendAsync(requestMessage)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    //using ConfigureAwait/...GetResult(), means I don't get AggregateExceptions as you do w/ .Result. nice.
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Unable to send {0} {1} request:\r\n{2}",
                        method, url, ex);
                    if (!string.IsNullOrEmpty(requestBody))
                        Logger.InfoFormat("The request body was:\r\n{0}", requestBody);
                    Logger.Error("The request was:\r\n" + requestMessage.ToString());
                    throw;
                }

                using (response)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.DebugFormat("Response: {0} - {1}",
                            (int) response.StatusCode, response.ReasonPhrase);
                    }
                    response.EnsureSuccessStatusCode();

                    string responseJson;
                    try
                    {
                        responseJson = response.Content.ReadAsStringAsync()
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("Unable to read {0} {1} response:\r\n{2}",
                            method, url, ex);
                        if(!string.IsNullOrEmpty(requestBody))
                            Logger.InfoFormat("The request body was: \r\n{0}", requestBody);
                        Logger.Error("The response was:\r\n" + response.ToString());
                        throw;
                    }
                    Logger.DebugFormat("Response: {0}", responseJson);

                    var result = typeof(T) == typeof(string)
                        ? responseJson as T
                        : responseJson.FromJson<T>();

                    return result;
                }
            }
        }

        private HttpContent CreateJsonStringContent(string requestBody)
        {
            if (string.IsNullOrEmpty(requestBody)) return null;

            return new StringContent(requestBody, Encoding.UTF8, ContentType.Json);
        }

        private HttpRequestMessage CreateJsonRequestMessage(HttpMethod method, string url, HttpContent content)
        {
            return new HttpRequestMessage(method, url)
            {
                Headers =
                {
                    //the server needs to compare the timestamp on the client
                    //against the timestamp associated w/ the Authentication ticket.
                    {TimestampHeaderName, DateTime.Now.ToUtcRfc1123()},
                    {"Accept", ContentType.Json}
                },
                Content = content
            };
        }

        /// <summary>
        ///     Issues a GET request to the specified URL, returning the response body as a string.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public string GetJson(string relativeUrl)
        {
            return GetJson<string>(relativeUrl);
        }

        /// <summary>
        ///     Issues a GET request to the specified URL, returning an instance of <see cref="TResponse" /> that has been
        ///     deserialized from JSON.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public TResponse GetJson<TResponse>(string relativeUrl) where TResponse : class
        {
            return SendJsonRequest<TResponse>(HttpMethod.Get, relativeUrl, string.Empty);
        }

        /// <summary>
        ///     Issues a POST request to the specified URL, returning the response body as a string.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string PostJson(string relativeUrl, object data)
        {
            return PostJson<string>(relativeUrl, data);
        }

        /// <summary>
        ///     Issues a POST request to the specified URL, returning an instance of <see cref="TResponse" /> that has been
        ///     deserialized from JSON.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TResponse PostJson<TResponse>(string relativeUrl, object data) where TResponse : class
        {
            return SendJsonRequest<TResponse>(HttpMethod.Post, relativeUrl, data.ToJson());
        }

        /// <summary>
        ///     Issues a PUT request to the specified URL, returning the response body as a string.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string PutJson(string relativeUrl, object data)
        {
            return PutJson<string>(relativeUrl, data);
        }

        /// <summary>
        ///     Issues a PUT request to the specified URL, returning an instance of <see cref="TResponse" /> that has been
        ///     deserialized from JSON.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TResponse PutJson<TResponse>(string relativeUrl, object data) where TResponse : class
        {
            return SendJsonRequest<TResponse>(HttpMethod.Put, relativeUrl, data.ToJson());
        }

        /// <summary>
        ///     Issues a DELETE request
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DeleteJson(string relativeUrl, object data)
        {
            return SendJsonRequest<string>(HttpMethod.Delete, relativeUrl, data.ToJson());
        }

        /// <summary>
        ///     Issues a DELETE request to the specified URL.
        /// </summary>
        /// <param name="relativeUrl"></param>
        public void Delete(string relativeUrl)
        {
            SendJsonRequest(HttpMethod.Delete, relativeUrl, string.Empty);
        }

        /// <summary>
        ///     Represents the response object for the Authenticate API method
        /// </summary>
        private class AuthenticateResponse
        {
            /// <summary>
            ///     The authentication ticket
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
    }
}