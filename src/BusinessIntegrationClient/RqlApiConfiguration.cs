using System;

namespace BusinessIntegrationClient
{
    public class RqlApiConfiguration
    {
        public RqlApiConfiguration()
        {
            UseSsl = true;
            RequestTimeout = TimeSpan.FromSeconds(30);
            Port = -1;
        }

        public string Site { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }


        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Specifies whether Ssl connection is required. Default to true.
        /// </summary>
        /// <remarks>
        /// This is only false in Local Dev environments
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
        ///     Optional UserAgent string to be added as the Http Header on all requests.
        /// </summary>
        public string UserAgent { get; set; }

    }
}