using System.Collections.Generic;

namespace BusinessIntegrationClient.Dtos
{
    public class Asset
    {
        /// <summary>
        ///     The unique Asset ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The Asset Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     The Asset Subtype.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        ///     The Asset Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     A place to provide custom properties not explicitly supported by members of this class.
        /// </summary>
        public Dictionary<string, string> ExtraInformation { get; set; }
    }
}