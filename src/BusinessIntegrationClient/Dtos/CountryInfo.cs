namespace BusinessIntegrationClient.Dtos
{
    public class CountryInfo
    {
        /// <summary>
        ///     The ISO Alpha-2 Country Code, a 2 character ISO standard for Countries. Example: US = United States of America
        /// </summary>
        public string CountryCode { get; set; }

        public string CountryName { get; set; }
    }
}