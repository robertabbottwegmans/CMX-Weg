using System;
using System.Collections.Generic;
using System.Linq;
using BusinessIntegrationClient.Dtos;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
{
    [TestFixture]
    public class WegmansUseCaseTests : BizApiTestFixtureBase
    {
        /// <summary>
        ///     User records starting w/ this user name prefix can be more easily be be deleted by scripts
        /// </summary>
        private const string UserNamePrefix = "unit.test.user_wegmans_";

        /// <summary>
        /// RetailLocation records w/ Ids starting with this can be more easily deleted by scripts
        /// </summary>
        private const string LocationIdPrefix = "TestRetailLocation_";

        private const string ExampleDivisionName = "Division 1";

        /// <summary>
        /// Contact records associated w/ the User that have this first name can be more easily deleted by scripts.
        /// </summary>
        private const string UnitTestFirstName = "Unit Test";

        private List<string> _allContactTypeIds;
        private List<string> _allProfileIds;
        private List<string> _allEntityTypeIds;

        private RetailLocation _sampleLocation;


        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _allContactTypeIds = _api.ListContactTypes()
                .Select(ct => ct.ContactTypeCode)
                .ToList();

            Assert.That(_allContactTypeIds, Is.Not.Null.And.Not.Empty,
                "Why didn't API return Contact Types?  See Admin->Core App Master Queues->Custom Dropdowns->Custom Dropdown Items, with Dropdown_Group_ID filter set to 'Contact Type'");

            _allProfileIds = _api.ListProfiles().Select(p => p.ProfileId).ToList();

            Assert.That(_allProfileIds, Is.Not.Null.And.Count.GreaterThan(1),
                "Why didn't API return Profiles? Ensure that Profile Records exist and that some are Active.  See the queue at Admin->Permissions Management->Profiles for the current site.");

            _allEntityTypeIds = _api.ListEntityTypes().Select(et => et.EntityTypeId)
                .ToList();

            Assert.That(_allEntityTypeIds, Is.Not.Null.And.Count.GreaterThan(1),
                "Why didn't API return Entity Types? See Admin->Core App Master Queues->Business Types");

            _sampleLocation = CreateSampleLocation();
        }

        [SetUp]
        public void Setup()
        {
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

        private RetailLocation CreateSampleLocation()
        {
            return new RetailLocation
            {
                Id = LocationIdPrefix + "sample_location",
                LocationName = "Sample Location",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main Street",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                MailingAddress = new Address
                {
                    Address1 = "PO BOX 12345",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                PrimaryPhoneNumber = "888-555-1212",
                FaxNumber = "888-555-1212",
                Concepts = null,
                ExtraInformation = new Dictionary<string, string>
                {
                    //Required: set a Division so corporate users can have access by division
                    {"Division", ExampleDivisionName}
                }
            };
        }

        [Test]
        public void LocationLevelUser_FoodSafetyCoordinator_ExampleUser()
        {
            var userName = UserNamePrefix + "food_safety_coordinator";

            //in this example, this user is a contact.
            //A user w/ contact info is considered a contact for each of 
            //the AssociatedEntities attached to the User.
            
            var profileId = "Location Manager";
            var contactType = "Food Safety Coordinator";
            var entityTypeId = "Retail Location";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@wegmans.com",
                FirstName = UnitTestFirstName,
                LastName = "User Name",
                PrimaryPhoneNumber = "800-555-1212",
                MobilePhoneNumber = "800-555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Happy St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                Title = "Food Safety Guy",
                ContactTypes = new List<string> {contactType},
                Profiles = new List<string> { profileId },

                AssociatedEntities = new List<EntityReference>
                {
                    //User will have location level data firewall for each associated entity:
                    //user will also be assigned as a contact for each 
                    //associated entity when the user info includes 
                    //contact fields such as title, address, etc.
                    new EntityReference(_sampleLocation.Id, entityTypeId)
                },
                SendNewUserNotification = false,//true?
            };

            Console.WriteLine(user.ToJson());
            //TODO: POST or PUT this user:
            //sanity checks:
            //APIs should have returned this lookup data
            //Assert.That(_allContactTypeIds, Has.Some.EqualTo(contactType));
            //Assert.That(_allProfileIds, Has.Some.EqualTo(profileId));
            //Assert.That(_allEntityTypeIds, Has.Some.EqualTo(entityTypeId));
            //_api.PostRetailLocation(_sampleLocation); //has to exist first
            //_api.PostUser(user);

        }

        [Test]
        public void LocationLevelUser_StoreManager_ExampleUser()
        {
            var userName = UserNamePrefix + "store_manager";

            //in this example, this user is a contact.
            //A user w/ contact info is considered a contact for each of 
            //the AssociatedEntities attached to the User.
            
            var profileId = "Location Manager";
            var contactType = "Store Manager";
            var entityTypeId = "Retail Location";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@wegmans.com",
                FirstName = UnitTestFirstName,
                LastName = "User Name",
                PrimaryPhoneNumber = "800-555-1212",
                MobilePhoneNumber = "800-555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Happy St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                Title = "Store Manager Guy",
                ContactTypes = new List<string> {contactType},
                Profiles = new List<string> { profileId },

                AssociatedEntities = new List<EntityReference>
                {
                    //User will have location level data firewall for each associated entity:
                    //user will also be assigned as a contact for each 
                    //associated entity when the user info includes 
                    //contact fields such as title, address, etc:
                    new EntityReference(_sampleLocation.Id, entityTypeId)
                },
                
                SendNewUserNotification = false,//true?
            };

            Console.WriteLine(user.ToJson());


            //TODO: POST or PUT this user:
            //sanity checks:
            //APIs should have returned this lookup data
            //Assert.That(_allContactTypeIds, Has.Some.EqualTo(contactType));
            //Assert.That(_allProfileIds, Has.Some.EqualTo(profileId));
            //Assert.That(_allEntityTypeIds, Has.Some.EqualTo(entityTypeId));
            //_api.PostRetailLocation(_sampleLocation); //has to exist first
            //_api.PostUser(user);

        }

        [Test]
        public void CorporateLevelUser_DivisionCoordinator_ExampleUser()
        {
            var userName = UserNamePrefix + "division_coordinator";

            //in this example, this user is a NOT a contact for the location
            //so no contact fields.

            var profileId = "Above Restaurant";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@wegmans.com",
                FirstName = UnitTestFirstName,
                LastName = "User Name",
                Profiles = new List<string> { profileId },
                //set/leave contact related fields null:
                PrimaryPhoneNumber = null,
                MobilePhoneNumber = null,
                PhysicalAddress = null,
                Title = null,
                ContactTypes = null,

                AssociatedEntities = null,
                AccessibleEntities = null,
                SendNewUserNotification = false, //true?

                Hierarchies = new Hierarchies
                {
                    //will create a data firewall where the user
                    //can see retail locations for matching divisions
                    Hierarchy = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            {"Division", ExampleDivisionName}
                        },
                        ////if has access to more than one division:
                        //new Dictionary<string, string>
                        //{
                        //    {"Division", "Division 2"}
                        //}
                    }
                }
            };

            Console.WriteLine(user.ToJson());
            //TODO: POST or PUT this user:
            //sanity checks:
            //APIs should have returned this lookup data
            //Assert.That(_allProfileIds, Has.Some.EqualTo(profileId));
            //_api.PostUser(user);
            //_api.PutUser(user);
        }

        [Test]
        public void CorporateLevelUser_FoodSafetyAssessor_ExampleUser()
        {
            var userName = UserNamePrefix + "food_safety_assessor";

            //in this example, this user is a NOT a contact for the location
            //so no contact fields.

            var profileId = "Auditor";
            var entityTypeId = "Retail Location";

            var user = new User
            {
                UserName = userName,
                Email = $"{userName}@wegmans.com",
                FirstName = UnitTestFirstName,
                LastName = "User Name",
                Profiles = new List<string> { profileId },
                //set/leave contact related fields null:
                PrimaryPhoneNumber = null,
                MobilePhoneNumber = null,
                PhysicalAddress = null,
                Title = null,
                ContactTypes = null,

                AssociatedEntities = null,
                AccessibleEntities = null,
                SendNewUserNotification = false, //true?

                //This user type has an open data firewall.
                HasAllAccess = true

            };

            Console.WriteLine(user.ToJson());
            //TODO: POST or PUT this user:
            //sanity checks:
            //APIs should have returned this lookup data
            //Assert.That(_allProfileIds, Has.Some.EqualTo(profileId));
            //Assert.That(_allEntityTypeIds, Has.Some.EqualTo(entityTypeId));
            //_api.PostUser(user);
            //_api.PutUser(user);
        }


        [TestCase("Division 1", RetailLocationTests.NumberOfTestRetailLocations, 
            Description = "Gets Records created by RetailLocationTests.CreateTestRetailLocations() based on the Division ExtraInformation value")]
        public void ListAllRetailLocations_ByDivision_CanGetByDivision(string division, int expectedNumber)
        {
            var result = _api.ListAllRetailLocations(rl => rl.ExtraInformation != null &&
                                                           rl.ExtraInformation["Division"] == division);

            Assert.That(result.Count, Is.GreaterThanOrEqualTo(expectedNumber));
    
        }
    }
}