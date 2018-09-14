using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BusinessIntegrationClient.Dtos;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
{
    [TestFixture]
    public class UserTests : BizApiTestFixtureBase
    {
        private const int NumberOfTestUsers = 5;

        private const string UserNamePrefix = "unit.test.user_";


        private const string TestUserName1 = "unit.test.user_1";
        private const string TestUserName2 = "unit.test.user_2";
        private const string TestUserName3 = "unit.test.user_3";
        private const string TestUserName4 = "unit.test.user_4";
        private const string TestUserName5 = "unit.test.user_5";

        /// <summary>
        ///     A key that is possible to pass as a Data Firewall Parameter when testing <see cref="User.AccessibleEntities" />.
        ///     The associated entity is expected to be a "Retail Location"
        /// </summary>
        /// <remarks>
        ///     This key has been defined in the Custom Attributes app.
        /// </remarks>
        private const string CustomAttributeKey_Division = "Division";

        /// <summary>
        /// A key that is possible to pass as a Data Firewall Parameter when testing <see cref="User.AccessibleEntities"/>. The associated entity is expected to be a "Retail Location"
        /// </summary>
        /// <remarks>
        ///     This key has been defined in the Custom Attributes app.
        /// </remarks>
        private const string CustomAttributeKey_District = "District";

        /// <summary>
        ///     A Profile Id for an Internal user profile type. See Permissions Management->Profiles
        /// </summary>
        /// <remarks>
        ///     Different Data Firewall rules can exist for internal/external user profile types.
        /// </remarks>
        private const string InternalProfileId = "Auditor";

        /// <summary>
        ///     A Profile Id for an Internal user profile type. See Permissions Management->Profiles
        /// </summary>
        /// <remarks>
        ///     Different Data Firewall rules can exist for internal/external user profile types.
        /// </remarks>
        private const string InternalProfileId2 = "Audit Manager";

        /// <summary>
        ///     A Profile Id for an External user profile type. See Permissions Management->Profiles
        /// </summary>
        /// <remarks>
        ///     Different Data Firewall rules can exist for internal/external user profile types.
        /// </remarks>
        private const string ExternalProfileId = "Location Manager";// "Supplier";

        /// <summary>
        ///     A Profile Id for an External user profile type. See Permissions Management->Profiles
        /// </summary>
        /// <remarks>
        ///     Different Data Firewall rules can exist for internal/external user profile types.
        /// </remarks>
        private const string ExternalProfileId2 = "Lab Tester";

        private List<string> _allContactTypeIds;
        private List<EntityReference> _allEntities;
        private List<string> _allProfileIds;

        private string _defaultProfileId;
        private string _defaultContactTypeId;
        private EntityReference _defaultEntity;
        private Address _defaultAddress;

        private void CreateTestUserAccounts()
        {
            var allUserNames = _api.ListAllUsers().Select(u => u.UserName).ToList();

            var users = Enumerable.Range(1, NumberOfTestUsers).Select(i =>
            {
                var userName = $"{UserNamePrefix}{i}";

                var extraInfoKey2 = $"StuffWithDifferentKey_{i}";
                var extraInfoValue2 = $"Value {i}";

                var user = new User
                {
                    UserName = userName,
                    Email = $"{userName}@compliancemetrix.com",
                    PrimaryPhoneNumber = "858-555-1212",
                    MobilePhoneNumber = "858-555-1212",
                    Title = "Sir",
                    FirstName = "Unit Test",
                    LastName = $"User Name {i}",
                    SendNewUserNotification = false,
                    HasAllAccess = true,
                    Profiles = new List<string> {_defaultProfileId},
                    ContactTypes = new List<string> {_defaultContactTypeId},
                    PhysicalAddress = _defaultAddress,
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"ExtraStuff", $"Extra Stuff Value {i}"},
                        {extraInfoKey2, extraInfoValue2}
                    },
                    Hierarchies = new Hierarchies
                    {
                        Hierarchy = 
                        {
                            new Dictionary<string, string>
                            {
                                {CustomAttributeKey_Division, "Division 1 NCAA" },
                                {"Level1-2", "Level 1.2 Value" }
                            },
                            new Dictionary<string, string>
                            {
                                {CustomAttributeKey_Division, "NCAA Division 1" }
                            }
                        }
                    }
                };

                if (!allUserNames.Contains(userName))
                {
                    var result = _api.PostUser(user);

                    Assert.That(result.UserName, Is.EqualTo(user.UserName));
                    Assert.That(result.Email, Is.EqualTo(user.Email));
                    Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
                    Assert.That(result.LastName, Is.EqualTo(user.LastName));
                    Assert.That(result.Title, Is.EqualTo(user.Title));
                    Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(user.PrimaryPhoneNumber));
                    Assert.That(result.MobilePhoneNumber, Is.EqualTo(user.MobilePhoneNumber));
                    Assert.That(result.Profiles, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.Profiles[0], Is.EqualTo(user.Profiles[0]));
                    Assert.That(result.HasAllAccess, Is.EqualTo(user.HasAllAccess));                    
                    Assert.That(result.ContactTypes, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.ContactTypes[0], Is.EqualTo(_defaultContactTypeId));
                    Assert.That(result.AssociatedEntities, Is.Null.Or.Empty);
                    Assert.That(result.AccessibleEntities, Is.Null.Or.Empty);

                    Assert.That(result.PhysicalAddress, Is.Not.Null);
                    Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(user.PhysicalAddress.Address1));
                    Assert.That(result.PhysicalAddress.Address2, Is.Null.Or.Empty);
                    Assert.That(result.PhysicalAddress.City, Is.EqualTo(user.PhysicalAddress.City));
                    Assert.That(result.PhysicalAddress.StateProvinceCode, Is.EqualTo(user.PhysicalAddress.StateProvinceCode));
                    Assert.That(result.PhysicalAddress.ZipCode, Is.EqualTo(user.PhysicalAddress.ZipCode));
                    Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(user.PhysicalAddress.CountryCode));

                    Assert.That(result.Hierarchies, Is.Not.Null);
                    Assert.That(result.Hierarchies.Hierarchy[0][CustomAttributeKey_Division], Is.Not.Null.And.Not.Empty);
                    Assert.That(result.Hierarchies.Hierarchy[1][CustomAttributeKey_Division], Is.Not.Null.And.Not.Empty);

                    Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.ExtraInformation[extraInfoKey2], Is.EqualTo(extraInfoValue2));
                }

                return user;

            }).ToList();

            Assert.That(users.Count, Is.EqualTo(NumberOfTestUsers));
        }

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _allProfileIds = _api.ListProfiles().Select(p => p.ProfileId).ToList();

            Assert.That(_allProfileIds, Is.Not.Null.And.Count.GreaterThan(1),
                "Ensure that Profile Records exist and that some are Active.  See the queue at Admin->Permissions Management->Profiles for the current site.");

            Assert.That(_allProfileIds, Has.Some.EqualTo(InternalProfileId),
                $"The database is missing the {InternalProfileId} profile Id.  Check Admin->Permission Management->Profiles");
            Assert.That(_allProfileIds, Has.Some.EqualTo(InternalProfileId2),
                $"The database is missing the {InternalProfileId2} profile Id.  Check Admin->Permission Management->Profiles");

            Assert.That(_allProfileIds, Has.Some.EqualTo(ExternalProfileId),
                $"The database is missing the {ExternalProfileId} profile Id.  Check Admin->Permission Management->Profiles");


            _defaultProfileId = _allProfileIds.Skip(1).First();

            //User Retail Locations because there are Custom Attributes defined for this organization/location type.. can use 
            var entities = _api.ListRetailLocations().Select(l => new EntityReference(l.Id, "Retail Location")).ToList();

            Assert.That(entities, Is.Not.Null.And.Not.Empty,
                "Ensure that the RetailLocationTests have executed first - this creates records that will be referenced here.");

            _allEntities = entities;
            _defaultEntity = entities.First();

            _allContactTypeIds = _api.ListContactTypes().Select(ct => ct.ContactTypeCode).ToList();

            Assert.That(_allContactTypeIds, Is.Not.Null.And.Not.Empty,
                "Where are the Contact Types?  See Admin->Core App Master Queues->Custom Dropdowns->Custom Dropdown Items, with Dropdown_Group_ID filter set to 'Contact Type'");

            _defaultContactTypeId = _allContactTypeIds.First();

            _defaultAddress = new Address
            {
                Address1 = "123 Main St",
                City = "San Diego",
                StateProvinceCode = "CA",
                ZipCode = "92109",
                CountryCode = "US"
            };

            CreateTestUserAccounts();
        }

        [SetUp]
        public void Setup()
        {
            _defaultAddress = new Address
            {
                Address1 = "123 Main St",
                City = "San Diego",
                StateProvinceCode = "CA",
                ZipCode = "92109",
                CountryCode = "US"
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {

        }

        #endregion

        [Test]
        public void GetJson_UserByUserName_GetsUserAsJsonString()
        {
            var getUserUrl = "Users/" + TestUserName1;

            var result = _api.GetJson<string>(getUserUrl);

            Assert.That(result, Is.Not.Null.And.Not.Empty);

            Assert.That(result, Is.StringContaining("UserName").And.StringContaining(TestUserName1));
        }

        [Test]
        public void GetJson_UserByUserName_GetsUserAsTypedDto()
        {
            var getUserUrl = "Users/" + TestUserName1;

            var result = _api.GetJson<User>(getUserUrl);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.UserName, Is.EqualTo(TestUserName1));
        }

        [Test]
        public void ListAllUsers_WithFilter_GetsAllMatchingFilter()
        {
            var result = _api.ListAllUsers(user => user.UserName == TestUserName1 ||
                                                   user.UserName == TestUserName3);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ListUsers_NoPaging_GetsAllUsers()
        {
            var users = _api.ListUsers(pageSize: -1);

            Assert.That(users, Is.Not.Null.And.Not.Empty);
            Assert.That(users.Count, Is.GreaterThanOrEqualTo(NumberOfTestUsers));
            Assert.That(users, Has.Some.Matches((UserSummary us) => us.UserName == TestUserName1));
            Assert.That(users, Has.None.Matches((UserSummary us) => us.UserName == "Anonymous"));
        }

        [Test]
        public void ListUsers_Page1Record_GetsOneUser()
        {
            var users = _api.ListUsers(pageSize: 1);

            Assert.That(users, Is.Not.Null.And.Not.Empty);
            Assert.That(users.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetUser_TestUser1_GetsUser()
        {
            var result = _api.GetUser(TestUserName1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(TestUserName1));
            Assert.That(result.Email, Is.Not.Null.And.Not.Empty);

            Assert.That(result.FirstName, Is.Not.Null.And.Not.Empty);
            Assert.That(result.LastName, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Title, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PrimaryPhoneNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.MobilePhoneNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Profiles[0], Is.EqualTo(_defaultProfileId));
            Assert.That(result.HasAllAccess, Is.True);
            Assert.That(result.ContactTypes[0], Is.EqualTo(_defaultContactTypeId));
            Assert.That(result.AssociatedEntities, Is.Null.Or.Empty);
            Assert.That(result.AccessibleEntities, Is.Null.Or.Empty);
            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.Address2, Is.Null.Or.Empty);
            Assert.That(result.PhysicalAddress.City, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.ZipCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.CountryCode, Is.Not.Null.And.Not.Empty);

            Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void GetUser_NoSuchUser_ThrowsException()
        {
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.GetUser("no such user name. 21-908i3-0941iir.109 i.0r..i");
            });

            Assert.That(ex.Message, Is
                .StringContaining("not found").IgnoreCase.Or
                .StringContaining("not exist").IgnoreCase);//IIS vs. self hosting result
        }

        [TestCase("CTM_625_retest_1")]
        [TestCase("sample.user")]
        [Explicit("These user names only exist in specific sites, may not match app.config")]
        public void GetUser_ByUserName_GetsUser(string userName)
        {
            var user = _api.GetUser(userName);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(userName));
            Assert.That(user.Email, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void PostUser_AlreadyExist_ThrowsException()
        {
            var user = _api.GetUser(TestUserName2);

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("already exist").IgnoreCase);
        }

        [TestCase(null, "noemail@compliancemetrix.com", "Test", "User")]
        [TestCase(UserNamePrefix + "missing_required_field_test_1", null, "Test", "User")]
        [TestCase(UserNamePrefix + "missing_required_field_test_1", "noemail@compliancemetrix.com", null, "User")]
        [TestCase(UserNamePrefix + "missing_required_field_test_1", "noemail@compliancemetrix.com", "Test", null)]
        public void PostUser_MissingRequiredFields_NonContactUser_ThrowsException(string userName, string email,
            string firstName, string lastName)
        {
            //This user isn't a contact, so not testing validation about address,etc.
            //if the user has lists of AssociatedEntities, address fields are required.
            var user = new User
            {
                UserName = userName,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var ex = Assert.Throws<HttpRequestException>(() => { _api.PostUser(user); });

            Assert.That(ex.Message, Is.StringContaining("not provided").IgnoreCase);
        }

        [TestCase(null, "San Diego", "CA", "US")]
        [TestCase("123 Main St", null,  "CA", "US")]
        [TestCase("123 Main St", "San Diego", "CA", null)]        
        [TestCase(null, null, null, null)]
        public void PostUser_MissingRequiredFields_IsContactUser_ThrowsException(
            string address, string city, string stateProvinceCode, string countryCode)
        {
            //This user isn't a contact, so not testing validation about address,etc.
            //if the user has lists of AssociatedEntities, address fields are required.

            var userName = UserNamePrefix + "ContactUserMissingRequiredFieldsUnitTest";
            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@compliancemetrix.com",
                FirstName = "Unit Test",
                LastName = "User Name",
                Title = "Sir",
                PrimaryPhoneNumber = "858-555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = address,
                    City = city,
                    StateProvinceCode = stateProvinceCode,
                    CountryCode = countryCode,
                },
                AssociatedEntities = new List<EntityReference> {_defaultEntity},
               
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided").IgnoreCase);
        }

        [Test]
        public void PostUser_AllMembersNull_ThrowsException()
        {
            var user = new User();

            var ex = Assert.Throws<HttpRequestException>(() => { _api.PostUser(user); });

            Assert.That(ex.Message, Is.StringContaining("UserName not provided").IgnoreCase
                .Or.StringContaining("Invalid Request").IgnoreCase);
        }

        [Test]
        public void PutUser_AddressHasNullStateProvinceCode_IsOk()
        {
            var user = _api.GetUser(TestUserName4);

            Assert.IsNotNull(user.PhysicalAddress);

            user.PhysicalAddress.Address1 = "123 Happy Street.";
            user.PhysicalAddress.City = "San Diego";
            user.PhysicalAddress.CountryCode = "US";
            user.PhysicalAddress.StateProvinceCode = null;

            var result = _api.PutUser(user);

            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(user.PhysicalAddress.Address1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(user.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(user.PhysicalAddress.CountryCode));
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.Null.Or.Empty);

        }

        [Test]
        public void PutUser_AddressToCountryWhereWeDontHaveStateCodes_IsOk()
        {

            var user = _api.GetUser(TestUserName4);

            Assert.IsNotNull(user.PhysicalAddress);

            user.PhysicalAddress.Address1 = "123 El Happy Street.";
            user.PhysicalAddress.City = "Buenas Aires";
            user.PhysicalAddress.CountryCode = "AR";
            user.PhysicalAddress.StateProvinceCode = "BA";

            var result = _api.PutUser(user);
            
            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(user.PhysicalAddress.Address1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(user.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(user.PhysicalAddress.CountryCode));
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.EqualTo(user.PhysicalAddress.StateProvinceCode));

        }

        [Test]
        public void PutUser_AddressToCountryWhereWeDontHaveStateCodes_HasStateProvinceName_WorksOk()
        {
            var user = _api.GetUser(TestUserName4);

            Assert.IsNotNull(user.PhysicalAddress);

            user.PhysicalAddress.City = "Buenes Aires";
            user.PhysicalAddress.Address2 = "This is a test line 2";
            user.PhysicalAddress.Address3 = "This is a test line 3";
            user.PhysicalAddress.CountryCode = "AR";
            
            user.PhysicalAddress.StateProvinceCode = "BA";//buenas aires


            var result = _api.PutUser(user);
            
            Assert.That(result.UserName, Is.EqualTo(user.UserName));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.Title, Is.EqualTo(user.Title));
            Assert.That(result.Profiles[0], Is.EqualTo(user.Profiles[0]));
            Assert.That(result.HasAllAccess, Is.EqualTo(user.HasAllAccess));
            Assert.That(result.MobilePhoneNumber, Is.EqualTo(user.MobilePhoneNumber));
            Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(user.PrimaryPhoneNumber));
            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(user.PhysicalAddress.Address1));
            Assert.That(result.PhysicalAddress.Address2, Is.EqualTo(user.PhysicalAddress.Address2));
            Assert.That(result.PhysicalAddress.Address3, Is.EqualTo(user.PhysicalAddress.Address3));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(user.PhysicalAddress.City));
        }

        [Test]
        public void PutUser_ChangeToExternalProfile_WithHasAllAccessTrue_ThrowsException()
        {
            var user = _api.GetUser(TestUserName3);

            Assert.That(user.Profiles, Is.Not.Null.And.Count.EqualTo(1));
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);


            //set to external profile id w/ "HasAllAccess" set to true.
            //validation should prevent it.
            user.Profiles = new List<string> {ExternalProfileId};
            user.HasAllAccess = true;

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("Cannot request All Access").IgnoreCase);
        }

        [Test]
        public void PutUser_SetExternalProfileId_WithHasAllAccessFalse_Works()
        {
            var user = _api.GetUser(TestUserName3);

            Assert.That(user.Profiles, Is.Not.Null.And.Count.EqualTo(1));
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);


            //set to external profile id w/ "HasAllAccess" set to false.
            //validation will allow it.
            user.Profiles = new List<string> {ExternalProfileId};
            user.HasAllAccess = false;


            var result = _api.PutUser(user);

            Assert.That(result.Profiles, Is.Not.Null.And.Count.EqualTo(1));
            Assert.That(result.Profiles[0], Is.EqualTo(ExternalProfileId));
            Assert.That(result.HasAllAccess, Is.Null.Or.False);

            var userAfterPut = _api.GetUser(result.UserName);

            Assert.That(userAfterPut.Profiles, Is.Not.Null.And.Count.EqualTo(1));
            Assert.That(userAfterPut.Profiles[0], Is.EqualTo(ExternalProfileId));
            Assert.That(userAfterPut.HasAllAccess, Is.Null.Or.False);
        }

        [Test]
        public void PutUser_ChangeToInternalProfile_WithHasAllAccessTrue_Works()
        {            
            var otherProfileId = InternalProfileId;

            var user = _api.GetUser(TestUserName3);

            Assert.That(user.Profiles, Is.Not.Null.And.Count.GreaterThanOrEqualTo(1));
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);

            var oldProfileId = user.Profiles[0];
            var newProfileId = oldProfileId == _defaultProfileId
                ? otherProfileId
                : _defaultProfileId;
            //toggle the profile Id between test runs
            user.Profiles = new List<string> {newProfileId};
            user.HasAllAccess = true;

            var result = _api.PutUser(user);

            Assert.That(result.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Profiles[0], Is.EqualTo(newProfileId));
            Assert.That(result.HasAllAccess, Is.True);

            var userAfterPut = _api.GetUser(result.UserName);

            Assert.That(userAfterPut.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(userAfterPut.Profiles[0], Is.EqualTo(newProfileId));
            Assert.That(userAfterPut.HasAllAccess, Is.True);

        }

        [Test]
        public void PutUser_ChangeProfile_SameIdTwice_ThrowsException()
        {
            var otherProfileId = _allProfileIds.Skip(2).Take(1).Single();

            var user = _api.GetUser(TestUserName4);

            Assert.That(user.Profiles, Is.Not.Null.And.Count.GreaterThanOrEqualTo(1));
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);

            
            //same profile twice
            user.Profiles = new List<string> {otherProfileId, otherProfileId};

            var ex = Assert.Throws<HttpRequestException>(()=>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("invalid profile").IgnoreCase);

        }

        [Test]
        public void PutUser_SetInternalAndExternalProfiles_CannotSetHasAllAccessTrue()
        {
            var user = _api.GetUser(TestUserName5);

            Assert.That(user.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);
            

            user.Profiles = new List<string>
            {
                _defaultProfileId,
                InternalProfileId,
                ExternalProfileId
            };
            user.HasAllAccess = true; //firewall rules prevent this by default for external user profile types.

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("Cannot request All Access").IgnoreCase);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void PutUser_Set2InternalProfiles_ChangesProfiles(bool hasAllAccess)
        {
            var otherProfileId = _allProfileIds.Skip(2).Take(1).Single();

            var user = _api.GetUser(TestUserName4);

            Assert.That(user.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(user.Profiles[0], Is.Not.Null.And.Not.Empty);

            user.Profiles = new List<string> {_defaultProfileId, otherProfileId};
            user.HasAllAccess = hasAllAccess;

            var result = _api.PutUser(user);

            Assert.That(result.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Profiles[0], Is.EqualTo(_defaultProfileId));
            Assert.That(result.Profiles[1], Is.EqualTo(otherProfileId));
            if (hasAllAccess)
                Assert.That(result.HasAllAccess, Is.True);
            else
                Assert.That(result.HasAllAccess, Is.Null.Or.False);

            var userAfterPut = _api.GetUser(result.UserName);

            Assert.That(userAfterPut.Profiles, Is.Not.Null.And.Not.Empty);
            Assert.That(userAfterPut.Profiles[0], Is.EqualTo(_defaultProfileId));
            Assert.That(userAfterPut.Profiles[1], Is.EqualTo(otherProfileId));
            if (hasAllAccess)
                Assert.That(result.HasAllAccess, Is.True);
            else
                Assert.That(result.HasAllAccess, Is.Null.Or.False);
        }

        [Test]
        public void PutUser_SetsAssociatedEntities_AssociatedEntitiesAreInResponse()
        {
            var user = _api.GetUser(TestUserName5);

            user.AssociatedEntities = _allEntities;
            user.AccessibleEntities = _allEntities;

            var result = _api.PutUser(user);

            Assert.That(result.AssociatedEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AssociatedEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[0].Id));
            Assert.That(result.AssociatedEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[1].Id));
            Assert.That(result.AssociatedEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[2].Id));

            Assert.That(result.AccessibleEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AccessibleEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[0].Id));
            Assert.That(result.AccessibleEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[1].Id));
            Assert.That(result.AccessibleEntities,
                Has.Some.Matches((EntityReference e) => e.Id == _allEntities[2].Id));

        }

        [Test]
        [Explicit(
            "This test expects two organzation to exists with specific Org ID")]
        public void PutUser_TwoCorporateForAssicatedEntities_ThrowsException()
        {
            var user = _api.GetUser(TestUserName5);

            user.AssociatedEntities = new List<EntityReference>()
            {
                new EntityReference("50876", "Supplier"),
                new EntityReference("70898", "Supplier")
            };
            user.AccessibleEntities = _allEntities;


            var ex = Assert.Throws<HttpRequestException>(() => { _api.PutUser(user); });

            Assert.That(ex.Message, Is.StringContaining("Cannot provided two corporate entities for a user").IgnoreCase);

        }

        [Test]
        [Explicit(
            "This test expects one organzation and one location that is the child of the organization")]
        public void PutUser_OneCorporateAndOneLocationForAssicatedEntities_ThrowsException()
        {
            var user = _api.GetUser(TestUserName5);

            user.AssociatedEntities = new List<EntityReference>()
            {
                new EntityReference("50876", "Supplier"),
                new EntityReference("05301", "Supplier Facility")
            };
            user.AccessibleEntities = _allEntities;


            var ex = Assert.Throws<HttpRequestException>(() => { _api.PutUser(user); });

            Assert.That(ex.Message, Is.StringContaining("One of the entity provided is not a child of another provided entity").IgnoreCase);



        }

        [Test]
        [Explicit(
            "This test expects one organzation and one location that is the child of the organization")]
        public void PutUser_OneCorporateAndOneLocationForAssicatedEntities_AssociatedEntitiesInResponse()
        {
            var user = _api.GetUser(TestUserName5);

            user.AssociatedEntities = new List<EntityReference>()
            {
                new EntityReference("16600", "Supplier"),
                new EntityReference("05301", "Supplier Facility")
            };
            user.AccessibleEntities = _allEntities;


            var result = _api.PutUser(user);

            Assert.That(result.AssociatedEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AssociatedEntities[0].Id, Is.EqualTo(user.AssociatedEntities[0].Id));
            Assert.That(result.AssociatedEntities[1].Id, Is.EqualTo(user.AssociatedEntities[1].Id));



        }

        [Test]
        public void PutUser_SetsOne_AssociatedAndAccessibleEntities_EntitiesAreInResponse()
        {
            var user = _api.GetUser(TestUserName5);

            var entities = _allEntities.Skip(2).Take(1).ToList();

            user.AssociatedEntities = entities;
            user.AccessibleEntities = entities;

            var result = _api.PutUser(user);

            Assert.That(result.AssociatedEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AssociatedEntities[0].Id, Is.EqualTo(entities[0].Id));
            Assert.That(result.AccessibleEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AccessibleEntities[0].Id, Is.EqualTo(entities[0].Id));
        }

        [Test]
        public void PutUser_WithExtraInformation_ExtraInfoIsReturned()
        {
            var user = _api.GetUser(TestUserName4);

            user.ExtraInformation = new Dictionary<string, string>
            {
                {CustomAttributeKey_Division, "NCAA Division 1"},
                {CustomAttributeKey_District, "District 9"},
                {"RandomKey", "Some value" }
            };

            var result = _api.PutUser(user);
            
            Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);

            Assert.That(result.ExtraInformation[CustomAttributeKey_Division],
                Is.EqualTo(user.ExtraInformation[CustomAttributeKey_Division]));

            Assert.That(result.ExtraInformation["RandomKey"],
                Is.EqualTo(user.ExtraInformation["RandomKey"]));
        }

        [Test]
        public void PostUser_DictionariesHaveInvalidXmlNameAsKey_ThrowsException()
        {
            var userName = $"{UserNamePrefix}InvalidXmlInExtraInfoTest";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@email.com",
                PhysicalAddress = _defaultAddress,
                FirstName = "Unit Test",
                LastName = "User Name",
                Profiles = new List<string> {_defaultProfileId}
            };

            var keyStartsWithNumber = "123_InvalidValidSomething";
            var keyHasStartingAngleBrackets = "<InvalidValidSomething>";
            var keyHasAngleBrackets = "InvalidValid<Something>";
            var keyHasApostrophes = "'InvalidValidSomething'>";
            var keyHasDoubleQuotes = "InvalidValid\"Something\"'>";
            var keyHasSpaces = "InvalidValid Something'";


            user.ExtraInformation = new Dictionary<string, string>
            {
                {"Valid-Name", "something"},
                {"Valid.Name", "something"}
            };
            user.Hierarchies = new Hierarchies
            {
                Hierarchy = 
                {
                    new Dictionary<string, string>
                    {
                        {"ValidSomethingHere", "valid something"}
                    },
                    new Dictionary<string, string>
                    {
                        {"ValidSomethingThere", "valid something"}
                    },
                    new Dictionary<string, string>
                    {
                        {keyStartsWithNumber, "invalid something"},
                        {keyHasStartingAngleBrackets, "invalid something"},
                        {keyHasAngleBrackets, "invalid something"},
                        {keyHasApostrophes, "invalid something"},
                        {keyHasDoubleQuotes, "invalid something"},
                        {keyHasSpaces, "invalid something"},
                    },
                }
            };
            var ex = Assert.Throws<Exception>(()=>
            {
                _api.PostUser(user);
            });

            Assert.That(ex.Message,
                Is.StringContaining(keyStartsWithNumber)
                    .And.StringContaining(keyHasStartingAngleBrackets)
                    .And.StringContaining(keyHasAngleBrackets)
                    .And.StringContaining(keyHasApostrophes)
                    .And.StringContaining(keyHasDoubleQuotes)
                    .And.StringContaining(keyHasSpaces));
        }

        [Test]
        public void PutUser_DictionariesHaveInvalidXmlNameAsKey_ThrowsException()
        {
            var userName = $"{UserNamePrefix}InvalidXmlInExtraInfoTest";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@email.com",
                PhysicalAddress = _defaultAddress,
                FirstName = "Unit Test",
                LastName = "User Name",
                Profiles = new List<string> {_defaultProfileId}
            };

            var keyStartsWithNumber = "123_InvalidValidSomething";
            var keyHasStartingAngleBrackets = "<InvalidValidSomething>";
            var keyHasAngleBrackets = "InvalidValid<Something>";
            var keyHasApostrophes = "'InvalidValidSomething'>";
            var keyHasDoubleQuotes = "InvalidValid\"Something\"'>";
            var keyHasSpaces = "Invalid Something'";


            user.ExtraInformation = new Dictionary<string, string>
            {
                {"Valid-Name", "something"},
                {"Valid.Name", "something"},
                {"ExtraInfo-"+ keyHasSpaces, "something"}
            };
            user.Hierarchies = new Hierarchies
            {
                Hierarchy = 
                {
                    new Dictionary<string, string>
                    {
                        {"ValidSomethingHere", "valid something"}
                    },
                    new Dictionary<string, string>
                    {
                        {"ValidSomethingThere", "valid something"}
                    },
                    new Dictionary<string, string>
                    {
                        {keyStartsWithNumber, "invalid something"},
                        {keyHasStartingAngleBrackets, "invalid something"},
                        {keyHasAngleBrackets, "invalid something"},
                        {keyHasApostrophes, "invalid something"},
                        {keyHasDoubleQuotes, "invalid something"},
                        {keyHasSpaces, "invalid something"},
                    },
                }
            };
            var ex = Assert.Throws<Exception>(()=>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message,
                Is.StringContaining("ExtraInfo-" + keyHasSpaces)
                    .And.StringContaining(keyHasStartingAngleBrackets)
                    .And.StringContaining(keyHasAngleBrackets)
                    .And.StringContaining(keyHasApostrophes)
                    .And.StringContaining(keyHasDoubleQuotes)
                    .And.StringContaining(keyHasSpaces));
        }

        [Test]
        public void VerifyKeysAreValidXmlNames_ExtraInfoHasInvalidNames_ThrowsException()
        {
            var invalidKeyName = "Invalid Something'";
            var user = new User
            {
                ExtraInformation = new Dictionary<string, string>
                {
                    {"Valid-Name", "something"},
                    {"Valid.Name", "something"},
                    {"ValidName", "something"},
                    {invalidKeyName, "something"},
                },
            };

            var ex = Assert.Throws<Exception>(() => user.VerifyKeysAreValidXmlNames());

            Assert.That(ex.Message, Is.StringContaining(invalidKeyName));
        }

        [Test]
        public void VerifyKeysAreValidXmlNames_HierarchiesHaveInvalidNames_ThrowsException()
        {
            var invalidKeyName = "Invalid Something'";
            var user = new User
            {
                Hierarchies = new Hierarchies
                {
                    Hierarchy = 
                    {
                        new Dictionary<string, string>
                        {
                            {"ValidSomethingHere", "valid something"}
                        },
                        new Dictionary<string, string>
                        {
                            {"ValidSomethingThere", "valid something"}
                        },
                        new Dictionary<string, string>
                        {
                            {invalidKeyName, "invalid something"},
                        },
                    }
                }
            };

            var ex = Assert.Throws<Exception>(() => user.VerifyKeysAreValidXmlNames());

            Assert.That(ex.Message, Is.StringContaining(invalidKeyName));
        }

        [Test]
        public void PostOrPutUser_NotAContact_WithExtraInfo_ReturnsExtraInfo()
        {
            var userName = $"{UserNamePrefix}NotAConcactWithExtraInfoTest";

            //This user is not a Contact, so it doesn't have Contact related fields
            //filled out.
            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@email.com",
                FirstName = "Unit Test",
                LastName = "Last Name",
                Profiles = new List<string> {_defaultProfileId},
                ExtraInformation = new Dictionary<string, string>
                {
                    {CustomAttributeKey_Division, "Pop Warner" },
                    {CustomAttributeKey_District, "District 7" },
                    {"ExtraInfo", "blah blah" }
                }
            };

            Assert.That(user.IsUserAlsoContact(), Is.False);

            var userExists = false;
            
            try
            {
                _api.GetUser(userName);
                userExists = true;
            }
            catch
            {
                //didn't exist
            }

            var result = userExists ? _api.PutUser(user) : _api.PostUser(user);

            Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
            Assert.That(result.ExtraInformation[CustomAttributeKey_Division], Is.EqualTo("Pop Warner"));
            Assert.That(result.ExtraInformation[CustomAttributeKey_District], Is.EqualTo("District 7"));
            Assert.That(result.ExtraInformation["ExtraInfo"], Is.EqualTo("blah blah"));


        }

        [Test]
        public void PutUser_AccessibleEntities_WithExtraInformation_ExtraInfoIsReturned()
        {
            var user = _api.GetUser(TestUserName5);

            var divisionValue = "Division 1";
            var entities = _allEntities.Skip(2).Take(1).Select(e =>
            {
                return new EntityReference(e.Id, e.EntityType)
                {
                    //The hierarchy values need to be configured and defined ahead of time
                    //as they relate to data firewall configuration for specific records.
                    //If you give it a value we don't expect, it won't get returned on a GET,PUT, or POST at this time
                    Hierarchy = new Dictionary<string, string>
                    {
                        {CustomAttributeKey_Division, divisionValue},
                        {"NotAHierarchy", "OtherValue"}//<-not a hierachy value, not gonna be returned.
                    }
                };
            }).ToList();

            user.AssociatedEntities = null;
            user.AccessibleEntities = entities;

            var result = _api.PutUser(user);

            Assert.That(result.AssociatedEntities, Is.Null.Or.Empty);

            Assert.That(result.AssociatedEntities, Is.Null);
            
            Assert.That(result.AccessibleEntities, Is.Not.Null.And.Not.Empty);
            Assert.That(result.AccessibleEntities[0].Id, Is.EqualTo(entities[0].Id));
            Assert.That(result.AccessibleEntities[0].Hierarchy, Is.Not.Null.And.Not.Empty,
                "The ExtraInformation was not returned. It should have been because this custom attribute has been defined in Core App Master Queues->Custom Attributes");

            Assert.That(result.AccessibleEntities[0].Hierarchy[CustomAttributeKey_Division], Is.EqualTo(divisionValue));
            
            Assert.That(result.AccessibleEntities[0].Hierarchy.ContainsKey("NotAHierarchy"), Is.False);

        }

        [TestCase("fake id", "Restaurant")]
        [TestCase("fake id", "fake entity type")]
        [TestCase(RestaurantTests.TestRestaurantId, "fake entity type")]
        public void PutUser_InvalidAssociatedEntities_ThrowsException(string id, string entityType)
        {
            var user = _api.GetUser(TestUserName5);

            user.AssociatedEntities = new List<EntityReference>
            {
                new EntityReference
                {
                    Id = id,
                    EntityType = entityType
                }
            };

            var ex = Assert.Throws<HttpRequestException>(()=>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is
                .StringContaining("not found").IgnoreCase.Or
                .StringContaining("not exist").IgnoreCase);//IIS vs. self hosting result

        }

        [TestCase("fake id", "Restaurant")]
        [TestCase("fake id", "fake entity type")]
        [TestCase(RestaurantTests.TestRestaurantId, "fake entity type")]
        public void PutUser_InvalidAccessibleEntities_ThrowsException(string id, string entityType)
        {
            var user = _api.GetUser(TestUserName2);

            user.AccessibleEntities = new List<EntityReference>
            {
                new EntityReference
                {
                    Id = id,
                    EntityType = entityType
                }
            };

            var ex = Assert.Throws<HttpRequestException>(()=>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is
                .StringContaining("not found").IgnoreCase.Or
                .StringContaining("not exist").IgnoreCase);//IIS vs. self hosting result
        }

        [Test]
        public void PutUser_AllMembersNull_ThrowsException()
        {
            var user = new User();

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("Not Allowed").IgnoreCase);
        }

        [Test]
        public void PutUser_NoSuchUser_ThrowsException()
        {
            var user = new User
            {
                UserName = UserNamePrefix + "no such user ids.df.2.3r.a.r3..",
                Email = $"{UserNamePrefix}nosuchuser@fake.com",
                FirstName = "Unit Test",
                LastName = "Unit Test",
                Profiles = new List<string> {_defaultProfileId},
                SendNewUserNotification = false
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is
                .StringContaining("not found").IgnoreCase.Or
                .StringContaining("not exist").IgnoreCase);//IIS vs. self hosting result
        }

        [TestCase(InternalProfileId, ExternalProfileId, Description = "This is a test w/ an Internal and an External User Profile Id")]        
        [TestCase(ExternalProfileId, ExternalProfileId2, Description = "This is a test w/ two External User Profile Ids")]
        public void PutUser_WhenExternalProfiles_HasAllAccessIsTrue_ThrowsException(string profileId1, string profileId2)
        {

            Assert.That(_allProfileIds, Has.Some.Matches(profileId1),
                $"The database is missing the {profileId1} profile Id.  Check Admin->Permission Management->Profiles");

            Assert.That(_allProfileIds, Has.Some.Matches(profileId2),
                $"The database is missing the {profileId2} profile Id.  Check Admin->Permission Management->Profiles");

            var userName = TestUserName5;

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@compliancemetrix.com",
                PrimaryPhoneNumber = "858-555-1212",
                MobilePhoneNumber = "858-555-1212",
                Title = "Sir",
                FirstName = "Unit Test",
                LastName = "User Name xyz",
                SendNewUserNotification = false,
                HasAllAccess = true,
                Profiles = new List<string> {profileId1, profileId2},

                ContactTypes = new List<string> {_defaultContactTypeId},
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                }
            };


            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutUser(user);
            });

            Assert.That(ex.Message, Is.StringContaining("Cannot request All Access").IgnoreCase);
        }

        [Test]
        [Explicit(
            "This test sends an email. Ensure Site Configuration is setup for testing. Manual Verification required")]
        public void PutUser_SendNewUserNotification_IsTrue_ShouldSendAnEmail()
        {
            var user = _api.GetUser(TestUserName4);

            user.SendNewUserNotification = true;

            var result = _api.PutUser(user);

            Assert.That(result.SendNewUserNotification, Is.Null);

            //TODO: go and check if the email got sent.
            
        }

        [Test]
        public void PutUser_SetHierarchies_ReturnsHierarchies()
        {
            var user = _api.GetUser(TestUserName1);

            user.Hierarchies = new Hierarchies
            {
                Hierarchy = 
                {
                    new Dictionary<string, string>
                    {
                        {CustomAttributeKey_Division, "NCAA Division 1"}
                    },
                    new Dictionary<string, string>
                    {
                        //note this key is in there twice, but at different levels.
                        {CustomAttributeKey_Division, "NCAA Division 2"},
                        {"NotAHierarchyKey", "Not a Hierarchy"}
                    }
                }
            };

            var result = _api.PutUser(user);

            Assert.That(result.Hierarchies, Is.Not.Null);
            Assert.That(result.Hierarchies.Hierarchy, Is.Not.Null.And.Not.Empty);

            //values aren't guaranteed to be returned in order we sent them, so don't depend on that in the asserts.
            Assert.That(result.Hierarchies[0][CustomAttributeKey_Division],
                Is.StringContaining("NCAA Division"));
            Assert.That(result.Hierarchies.Hierarchy[0][CustomAttributeKey_Division],
                Is.StringContaining("NCAA Division"));

            Assert.That(result.Hierarchies[1][CustomAttributeKey_Division],
                Is.StringContaining("NCAA Division"));
            Assert.That(result.Hierarchies.Hierarchy[1][CustomAttributeKey_Division],
                Is.StringContaining("NCAA Division"));
            var allKeys = result.Hierarchies.Hierarchy.SelectMany(h => h.Keys).ToList();

            Assert.That(allKeys.Any(k => k == "NotAHierarchyKey"), Is.False,
                "Keys that are not part of Hierarchies are not returned at this time, but if that changes, that's probably ok & not a reason to fail the test.");
        }

        [Test]
        [Explicit("This test deletes a record, however we can't undelete a record. The Id may not be reused after deleting it.")]
        public void DeleteUser_CreateAndAndDeleteIt_CannotGetAndCannotList()
        {
            var userName = $"{UserNamePrefix}For_Deletion_3"; //increment the # after succesful test runs, or delete records w/ a RQLCMD script-(see QueryStores + DelStores)

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@compliancemetrix.com",
                PrimaryPhoneNumber = "858-555-1212",
                MobilePhoneNumber = "858-555-1212",
                Title = "Sir",
                FirstName = "Unit Test",
                LastName = "User Name For Deletion",
                SendNewUserNotification = false,
                HasAllAccess = false,
                Profiles = new List<string> { _defaultProfileId },
                ContactTypes = new List<string> { _defaultContactTypeId },
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                },
            };


            var userToDelete = _api.PostUser(user);

            var userAfterPost = _api.GetUser(userName);

            Assert.IsNotNull(userAfterPost, "Should be able to GET an asset after POSTing it...");

            var allUsersAfterPost = _api.ListUsers();

            Assert.That(allUsersAfterPost.Select(a => a.UserName).ToList(),
                Has.Some.Contains(userToDelete.UserName));

            _api.DeleteUser(userName);

            //should get error 404 - not found
            var ex = Assert.Throws<HttpRequestException>(() => _api.GetUser(userName));

            Assert.That(ex.Message, Is.StringContaining("404"));

            //should not be listed either
            var allUsersAfterDelete = _api.ListUsers();

            Assert.That(allUsersAfterDelete.Select(a => a.UserName).ToList(),
                Has.None.Contains(userName));
        }


        [Test]
        public void Hierarchies_CanUseIndexer()
        {
            var hierarchies = new Hierarchies(3);

            var special = new Dictionary<string, string>
            {
                {"Key1", "Value2"}
            };

            hierarchies[1].Add("Key1", "Value1");
            hierarchies[2]["Key1"] = hierarchies[1]["Key1"];

            var result = hierarchies[2]["Key1"];

            Assert.That(result, Is.EqualTo("Value1"));

            hierarchies[2] = special;

            result = hierarchies[2]["Key1"];

            Assert.That(result, Is.EqualTo("Value2"));
        }
    }
}