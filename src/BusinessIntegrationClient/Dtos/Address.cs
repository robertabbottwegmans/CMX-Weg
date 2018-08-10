namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    ///     Represent an Address.
    /// </summary>
    public class Address
    {
        /// <summary>
        ///     This is the "Address Line 1" value in an address. Required.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        ///     Optional Address Line 2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        ///     Optional Address Line 3
        /// </summary>
        public string Address3 { get; set; }

        /// <summary>
        ///     The City name - required.
        /// </summary>
        /// <remarks>
        ///     This is a required field
        /// </remarks>
        public string City { get; set; }

        /// <summary>
        ///     The 2 or 3 character state code, or Full state/province name
        /// </summary>
        public string StateProvinceCode { get; set; }

        public string ZipCode { get; set; }

        /// <summary>
        ///     The ISO Alpha-2 Country Code, a 2 character ISO standard for Countries. Example: US = United States
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         For a list of country codes, <see cref="http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2" />
        ///     </para>
        ///     <para>
        ///         This is a required field
        ///     </para>
        /// </remarks>
        public string CountryCode { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address1) &&
                   string.IsNullOrEmpty(Address2) &&
                   string.IsNullOrEmpty(Address3) &&
                   string.IsNullOrEmpty(City) &&
                   string.IsNullOrEmpty(StateProvinceCode) &&
                   string.IsNullOrEmpty(CountryCode);
        }
    }
}