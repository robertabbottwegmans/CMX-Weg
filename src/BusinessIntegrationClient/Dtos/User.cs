using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BusinessIntegrationClient.Dtos
{
    public class User
    {
        /// <summary>
        ///     The unique user name for this user. Required.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     The Email Address associated with this user. Required.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The user's first name. Required.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The user's last name. Required.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     When the user is also a Contact, this is the Contact Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     User's Primary Phone #. Required if the user is a Contact
        /// </summary>
        public string PrimaryPhoneNumber { get; set; }

        public string MobilePhoneNumber { get; set; }

        /// <summary>
        ///     The User's Physical Address. If this user is has <see cref="AssociatedEntities" />, this is required, as well as
        ///     the required address fields.
        /// </summary>
        public Address PhysicalAddress { get; set; }

        /// <summary>
        ///     If this User is also a Contact for an organization, this specifies the type(s) of Contact this user is.   See
        ///     <see cref="BusinessApiExtensions.ListContactTypes" /> to get a list of valid <see cref="ContactTypes" />, using the
        ///     value from <see cref="ContactType.ContactTypeCode" /> here.
        /// </summary>
        public List<string> ContactTypes { get; set; }

        /// <summary>
        ///     If this User is also a Contact for an organization, this specifies a list of references to the associated entities,
        ///     such as Suppliers, Distributers, etc.
        ///     See <see cref="BusinessApiExtensions.ListEntityTypes" /> for a list of valid <see cref="EntityTypeInfo" />.
        ///     <see cref="EntityReference.Id" /> should be set to a value from <see cref="EntityTypeInfo.EntityTypeId" />,
        ///     <see cref="EntityReference.EntityType" /> should be set from <see cref="EntityTypeInfo.EntityTypeName" />.
        /// </summary>
        /// <remarks>
        ///     This is used by the platform to restrict visible records via a Data Firewall
        /// </remarks>
        public List<EntityReference> AssociatedEntities { get; set; }

        /// <summary>
        ///     Controls which entities are accessible to User and helps initialize the User's Data Firewall.
        ///     For a list if entities,      See <see cref="BusinessApiExtensions.ListEntityTypes" /> for a list of valid
        ///     <see cref="EntityTypeInfo" />.
        ///     <see cref="EntityReference.Id" /> should be set to a value from <see cref="EntityTypeInfo.EntityTypeId" />,
        ///     <see cref="EntityReference.EntityType" /> should be set from <see cref="EntityTypeInfo.EntityTypeName" />.
        /// </summary>
        /// <remarks>
        ///     <para>The referenced entities need to exist prior to referencing them here.</para>
        ///     <para>This is used by the platform to restrict visible records via a Data Firewall</para>
        /// </remarks>
        public List<EntityReference> AccessibleEntities { get; set; }

        /// <summary>
        ///     A list of Profile identifiers that this user has membership in.
        /// </summary>
        /// <remarks>
        ///     The Profile Id is a string value that is the key to a record in the Profiles app.
        ///     <see cref="BusinessApiExtensions.ListProfiles" />
        /// </remarks>
        public List<string> Profiles { get; set; }

        /// <summary>
        ///     When creating a user, setting this to true will cause the new user email notification to be sent, otherwise no
        ///     email is sent.
        /// </summary>
        public bool? SendNewUserNotification { get; set; }

        /// <summary>
        ///     Indicates this user has complete Data Firewall access.
        /// </summary>
        public bool? HasAllAccess { get; set; }


        /// <summary>
        ///     A place to provide custom properties not explicitly supported by members of this class.
        /// </summary>
        public Dictionary<string, string> ExtraInformation { get; set; }

        /// <summary>
        ///     Heirarchies are used to configure the data firewall for this user. The data firewall controls which records a user
        ///     is eligible to view.
        /// </summary>
        public Hierarchies Hierarchies { get; set; }


        /// <summary>
        ///     Determines if this instance of <see cref="User" /> is also a contact for an organization.  Users are considered a
        ///     contact if any of the fields typically associated with a Contact record are present.
        /// </summary>
        public bool IsUserAlsoContact()
        {
            return AssociatedEntities != null && AssociatedEntities.Any() ||
                   ContactTypes != null && ContactTypes.Any() ||
                   !string.IsNullOrEmpty(PrimaryPhoneNumber) ||
                   !string.IsNullOrEmpty(MobilePhoneNumber) ||
                   !string.IsNullOrEmpty(Title) ||
                   PhysicalAddress != null && !PhysicalAddress.IsEmpty();
        }

        /// <summary>
        ///     The values of any Dictionary keys need to be XML friendly characters.
        ///     This is because this DTO is converted to XML by the platform and Dictionary keys become Xml elements.  Only valid
        ///     Xml characters may be used.
        /// </summary>
        public void VerifyKeysAreValidXmlNames()
        {
            var invalidKeyNames = new List<string>();
            var invalidKeyReasons = new List<string>();
            if (ExtraInformation != null)
                invalidKeyNames.AddRange(ExtraInformation.Keys.Where(key => !IsXmlFriendlyName(key, invalidKeyReasons))
                    .ToList());

            if (Hierarchies != null)
                invalidKeyNames.AddRange(Hierarchies.Hierarchy.SelectMany(dict =>
                    dict.Keys.Where(key => !IsXmlFriendlyName(key, invalidKeyReasons))));

            if (invalidKeyNames.Any())
            {
                var message =
                    $"This User instance contains Dictionary Key names that cannot not serialized as valid xml element names and cannot be submitted.  Invalid Key Names: {string.Join(",", invalidKeyNames)}{Environment.NewLine}{string.Join(Environment.NewLine, invalidKeyReasons)}";

                throw new Exception(message);
            }
        }

        private bool IsXmlFriendlyName(string name, List<string> reasons)
        {
            try
            {
                name = XmlConvert.VerifyName(name);
                return true;
            }
            catch (XmlException ex)
            {
                reasons.Add(ex.Message);
                return false;
            }
        }
    }
}