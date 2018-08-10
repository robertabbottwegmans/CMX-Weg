using System.Collections.Generic;

namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    ///     Represents an Retail Location within the site.
    /// </summary>
    public class RetailLocation
    {
        /// <summary>
        ///     The Unique Location Id(Required).
        /// </summary>
        /// <remarks>
        /// This Id must be unique accross all organization locations(Restaurants, RetailLocations, etc).
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Required Location Name
        /// </summary>
        public string LocationName { get; set; }


        public string PrimaryPhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        /// <summary>
        /// Required Physical Address of the RetailLocation
        /// </summary>
        public Address PhysicalAddress { get; set; }

        /// <summary>
        /// An optional Mailing Address for this RetailLocation
        /// </summary>
        public Address MailingAddress { get; set; }

        /// <summary>
        /// A list of Concept Ids associated with this RetailLocation.
        /// <see cref="BusinessApiExtensions.ListConcepts"/> and <see cref="ConceptInfo.ConceptId"/>
        /// </summary>
        public List<string> Concepts { get; set; }


        /// <summary>
        ///     A place to provide custom properties not explicitly supported by members of this class.
        /// </summary>
        public Dictionary<string, string> ExtraInformation { get; set; }
    }
}