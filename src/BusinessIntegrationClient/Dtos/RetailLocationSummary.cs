using System.Collections.Generic;

namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    /// Represents one row of Retail Location data from a queue.
    /// </summary>
    /// <remarks>
    /// When listing all records, data is typically 'flattened', as data is read from a table w/ columns
    /// </remarks>
    public class RetailLocationSummary
    {
        public string Id { get; set; }
        public string LocationName { get; set; }
        public string ConceptIds { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public string PhysicalZipCode { get; set; }
        public string PhysicalCountry { get; set; }
        public string MailingAddress1 { get; set; }
        public string MailingAddress2 { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingZipCode { get; set; }
        public string MailingCountry { get; set; }

        public Dictionary<string,string> ExtraInformation { get; set; }
    }
}