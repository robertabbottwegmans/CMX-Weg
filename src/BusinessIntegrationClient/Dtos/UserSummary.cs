namespace BusinessIntegrationClient.Dtos
{
    /// <summary>
    ///     The POCO returned when listing Users.  For full details, make a seperate request per user.
    /// </summary>
    public class UserSummary
    {
        /// <summary>
        ///     The unique user name for this user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     The Email Address associated with this user.
        /// </summary>
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        ///     Comma seperated list of profile ids.
        /// </summary>
        public string ProfileIds { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string StateProvince { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string PrimaryPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}