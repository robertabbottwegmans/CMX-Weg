using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using BusinessIntegrationClient.Dtos;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester

{
    [TestFixture]
    public class RestaurantTests : BizApiTestFixtureBase
    {
        private const int NumberOfTestRestaurants = 3;

        /// <summary>
        /// Use this prefix for all Restuarant Ids so it can be more easily deleted w/ a script.
        /// </summary>
        private const string TestRestuarantIdPrefix = "TestRestaurant_";
        internal const string TestRestaurantId = "TestRestaurant_1";
        private const string TestRestaurantId2 = "TestRestaurant_2";
        private const string TestRestaurantId3 = "TestRestaurant_3";


        private void CreateTestRestaurants()
        {
            var allRestaurantIds = _api.ListRestaurants()
                .Select(a => a.Id)
                .ToList();

            var restuarants = Enumerable.Range(1, NumberOfTestRestaurants).Select(i =>
            {
                var retailLocationId = $"{TestRestuarantIdPrefix}{i}";

                var extraInfoKey2 = $"StuffWithDifferentKey_{i}";
                var extraInfoValue2 = $"Value {i}";
                var restaurant = new Restaurant
                {
                    Id = retailLocationId,
                    LocationName = $"Test Restaurant {i}",
                    PrimaryPhoneNumber = "858-555-1212",
                    FaxNumber = "555-1212",
                    PhysicalAddress = new Address
                    {
                        Address1 = "123 Main St",
                        City = "San Diego",
                        StateProvinceCode = "CA",
                        ZipCode = "92109",
                        CountryCode = "US"
                    },
                    MailingAddress = new Address
                    {
                        Address1 = "PO BOX 123456",
                        City = "San Diego",
                        StateProvinceCode = "CA",
                        ZipCode = "92109",
                        CountryCode = "US"
                    },
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"ExtraStuff", $"Extra Stuff Value {i}"},
                        {extraInfoKey2, extraInfoValue2}
                    }
                };
                if (!allRestaurantIds.Contains(retailLocationId))
                {
                    var result = _api.PostRestaurant(restaurant);
                  //  var result = _api.PutRetailLocation(location);

                    Assert.That(result.Id, Is.EqualTo(restaurant.Id));
                    Assert.That(result.LocationName , Is.EqualTo(restaurant.LocationName));
                    Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(restaurant.PrimaryPhoneNumber));
                    Assert.That(result.FaxNumber, Is.EqualTo(restaurant.FaxNumber));
                    Assert.That(result.PhysicalAddress, Is.Not.Null);
                    Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(restaurant.PhysicalAddress.Address1));
                    Assert.That(result.PhysicalAddress.Address2, Is.Null.Or.Empty);
                    Assert.That(result.PhysicalAddress.City, Is.EqualTo(restaurant.PhysicalAddress.City));
                    Assert.That(result.PhysicalAddress.StateProvinceCode, Is.EqualTo(restaurant.PhysicalAddress.StateProvinceCode));
                    Assert.That(result.PhysicalAddress.ZipCode, Is.EqualTo(restaurant.PhysicalAddress.ZipCode));
                    Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(restaurant.PhysicalAddress.CountryCode));
                    Assert.That(result.MailingAddress, Is.Not.Null);
                    Assert.That(result.MailingAddress.Address1, Is.EqualTo(restaurant.MailingAddress.Address1));
                    Assert.That(result.MailingAddress.Address2, Is.Null.Or.Empty);
                    Assert.That(result.MailingAddress.Address3, Is.Null.Or.Empty);
                    Assert.That(result.MailingAddress.City, Is.EqualTo(restaurant.MailingAddress.City));
                    Assert.That(result.MailingAddress.StateProvinceCode, Is.EqualTo(restaurant.MailingAddress.StateProvinceCode));
                    Assert.That(result.MailingAddress.ZipCode, Is.EqualTo(restaurant.MailingAddress.ZipCode));
                    Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(restaurant.MailingAddress.CountryCode));

                    Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.ExtraInformation[extraInfoKey2], Is.EqualTo(extraInfoValue2));
                    Assert.That(result.ExtraInformation.ContainsKey("ExtraStuff"), Is.True);
                }

                return restaurant;
            }).ToList();


            Assert.That(restuarants.Count, Is.EqualTo(NumberOfTestRestaurants));
        }

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            CreateTestRestaurants();
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

        [Test]
        public void ListAllRestaurants_WithFilter_GetsAllMatchingFilter()
        {
            var result = _api.ListAllRestaurants(r => r.Id == TestRestaurantId ||
                                                      r.Id == TestRestaurantId2);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ListRestaurants_NoPaging_ListsAll()
        {
            var result = _api.ListRestaurants();
            
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(NumberOfTestRestaurants));
        }

        [Test]
        public void ListRestaurants_PageSize1_Gets1()
        {
            var result = _api.ListRestaurants(pageSize: 1);

            Assert.That(result.Count, Is.EqualTo(1));
        }


        [Test]
        public void GetRestaurant_ById_GetsIt()
        {
            var result = _api.GetRestaurant(TestRestaurantId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(TestRestaurantId));
            Assert.That(result.LocationName, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PrimaryPhoneNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.FaxNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.Address2, Is.Null.Or.Empty);
            Assert.That(result.PhysicalAddress.Address3, Is.Null.Or.Empty);
            Assert.That(result.PhysicalAddress.City, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.ZipCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.CountryCode, Is.Not.Null.And.Not.Empty);

            Assert.That(result.MailingAddress, Is.Not.Null);
            Assert.That(result.MailingAddress.Address1, Is.Not.Null.And.Not.Empty);
            Assert.That(result.MailingAddress.Address2, Is.Null.Or.Empty);
            Assert.That(result.MailingAddress.Address3, Is.Null.Or.Empty);
            Assert.That(result.MailingAddress.City, Is.Not.Null.And.Not.Empty);            
            Assert.That(result.MailingAddress.StateProvinceCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.MailingAddress.ZipCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result.MailingAddress.CountryCode, Is.Not.Null.And.Not.Empty);

            Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
            Assert.That(result.ExtraInformation.ContainsKey("ExtraStuff"), Is.True,
                "This record was created w/ a key named ExtraStuff.. where is it now?");
        }

        [Test]
        public void GetRestaurant_NoSuchId_ThrowsException()
        {
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.GetRestaurant("This Id doesn't exist");
            });

            Assert.That(ex.Message, Is.StringContaining("not exist").IgnoreCase);
        }

        [Test]
        public void PostRestaurant_AlreadyExists_ThrowsException()
        {
            var restaurant = _api.GetRestaurant(TestRestaurantId);

            Assert.IsNotNull(restaurant);

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                //insert records w/ POST, will throw if already exists.
                //Use PUT to update.
                _api.PostRestaurant(restaurant);
            });

            Assert.That(ex.Message, Is.StringContaining("already exist").IgnoreCase);
        }

        [TestCase("USSA!", "US")]
        [TestCase("US", "USSA!")]
        public void PostRestaurant_InvalidCountryCode_ThrowsException(string physicalAddressCountryCode, string mailingAddressCountryCode)
        {
            var restaurant = new Restaurant
            {
                Id = "InvalidCountryCodeTest",
                LocationName = "Invalid Country Code Test",
                PrimaryPhoneNumber = "858-555-1212",
                FaxNumber = "555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = physicalAddressCountryCode
                },
                MailingAddress = new Address
                {
                    Address1 = "PO BOX 123456",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = mailingAddressCountryCode
                }
            };
           
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                //insert records w/ POST, will throw if already exists.
                //Use PUT to update.
                _api.PostRestaurant(restaurant);
            });

            Assert.That(ex.Message, Is.StringContaining("not a valid").IgnoreCase);
        }

        [TestCase("CALI!", "CA")]
        [TestCase("CA", "CALI!")]
        public void PostRestaurant_InvalidStateProviceCode_ThrowsException(string physicalAddressStateProvinceCode, string mailingAddressStateProvinceCode)
        {
            var restaurant = new Restaurant
            {
                Id = "InvalidStateProvinceCodeTest",
                LocationName = "Invalid StateProvince Code Test",
                PrimaryPhoneNumber = "858-555-1212",
                FaxNumber = "555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = physicalAddressStateProvinceCode,
                    ZipCode = "92109",
                    CountryCode = "US"
                },
                MailingAddress = new Address
                {
                    Address1 = "PO BOX 123456",
                    City = "San Diego",
                    StateProvinceCode = mailingAddressStateProvinceCode,
                    ZipCode = "92109",
                    CountryCode = "US"
                }
            };

           
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                //insert records w/ POST, will throw if already exists.
                //Use PUT to update.
                _api.PostRestaurant(restaurant);
            });

            Assert.That(ex.Message, Is.StringContaining("is not a valid").IgnoreCase);
        }

        [Test]
        public void PutRestaurant_NoSuchId_ThrowsException()
        {
            var location = new Restaurant
            {
                Id = "No such Id 123k1-094-0`i8 ]9t0pj3pvkmas ;",
                LocationName = "Name Of Location that has Id that doesn't exist",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                }
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRestaurant(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not exist").IgnoreCase);
        }


        [Test]
        public void PutRestaurant_ExistingAsset_UpdatesAsset()
        {
            var restaurant = _api.GetRestaurant(TestRestaurantId);

            Assert.IsNotNull(restaurant);

            //toggle this value each time the test is run...
            var newValue = restaurant.PrimaryPhoneNumber == "858-555-1212"
                ? "+1-858-555-1212"
                : "858-555-1212";

            restaurant.PrimaryPhoneNumber = newValue;

            var result = _api.PutRestaurant(restaurant);

            Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(newValue));

            var result2 = _api.GetRestaurant(TestRestaurantId);

            Assert.That(result2.PrimaryPhoneNumber, Is.EqualTo(newValue));
        }

        [TestCase(null, "123 Main St.", "San Diego", "US")]
        [TestCase("Test", null, "San Diego", "US")]
        [TestCase("Test", "123 Main St.", null, "US")]
        [TestCase("Test", "123 Main St.", "San Diego", null)]
        [TestCase(null, null, null, null)]
        public void PutRestaurant_UpdateExistingWithMissingRequiredFields_ThrowsException(string name, string address1,
            string city, string countryCode)
        {
            var location = _api.GetRestaurant(TestRestaurantId);

            location.LocationName = name;
            location.PhysicalAddress.Address1 = address1;
            location.PhysicalAddress.City = city;
            location.PhysicalAddress.CountryCode = countryCode;

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRestaurant(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        public void PutRestaurant_AddressInCountryWithoutStateCodesInDB_IsOk()
        {
            var restaurant = _api.GetRestaurant(TestRestaurantId2);

            Assert.That(restaurant.PhysicalAddress, Is.Not.Null);
            restaurant.PhysicalAddress.Address1 = "123 Somewhere st.";
            restaurant.PhysicalAddress.City = "Buenas Aires";
            restaurant.PhysicalAddress.StateProvinceCode = "BA";//<-- we don't have Argentine provinces in our DB
            restaurant.PhysicalAddress.CountryCode = "AR";

            restaurant.MailingAddress = new Address
            {
                Address1 = "123 Somewhere st.",
                City = "Buenas Aires",
                StateProvinceCode = "BA",
                CountryCode = "AR"
            };

            var result = _api.PutRestaurant(restaurant);

            Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(restaurant.PhysicalAddress.Address1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(restaurant.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.EqualTo(restaurant.PhysicalAddress.StateProvinceCode));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(restaurant.PhysicalAddress.CountryCode));

            Assert.That(result.MailingAddress.Address1, Is.EqualTo(restaurant.MailingAddress.Address1));
            Assert.That(result.MailingAddress.City, Is.EqualTo(restaurant.MailingAddress.City));
            Assert.That(result.MailingAddress.StateProvinceCode, Is.EqualTo(restaurant.MailingAddress.StateProvinceCode));
            Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(restaurant.MailingAddress.CountryCode));
        }

        [Test]
        public void PutRestaurant_UpdateExisting_NoPhysicalAddress_ThrowsException()
        {
            var location = _api.GetRestaurant(TestRestaurantId);

            Assert.That(location.PhysicalAddress, Is.Not.Null);

            location.PhysicalAddress = null;

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRestaurant(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        [Explicit("This test should be run after a Concept w/ Id UnitTestConceptId has been manually created")]
        public void PutRestaurant_ValidConceptId_Works()
        {
            var location = _api.GetRestaurant(TestRestaurantId2);

            location.Concepts = new List<string> {UnitTestConceptId};

            var result = _api.PutRestaurant(location);

            Assert.That(result.Concepts, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Concepts[0], Is.EqualTo(location.Concepts[0]));

        }

        [Test]
        [Explicit("This test should be run after a Concept w/ Id UnitTestConceptId and UnitTestConceptId2 has been manually created")]
        public void PutRestaurant_TwoValidConceptIds_Works()
        {
            var location = _api.GetRestaurant(TestRestaurantId2);

            location.Concepts = new List<string> {UnitTestConceptId, UnitTestConceptId2};

            var result = _api.PutRestaurant(location);

            Assert.That(result.Concepts, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Concepts[0], Is.EqualTo(location.Concepts[0]));
            Assert.That(result.Concepts[1], Is.EqualTo(location.Concepts[1]));

        }

        [Test]
        public void PostRestaurant_InvalidConceptId_ThrowsException()
        {
            var restaurant = new Restaurant
            {
                Id = "RestaurantWithInvalidConceptIdTest",
                LocationName = "Invalid Concept Id test",
                PrimaryPhoneNumber = "858-555-1212",
                FaxNumber = "555-1212",
                ExtraInformation = new Dictionary<string, string>
                {
                    {"Lots", "OfStuff"}
                },
                PhysicalAddress = new Address
                {
                    Address1 = "PO Box 12345",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                MailingAddress = new Address
                {
                    Address1 = "PO Box 12345",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                Concepts = new List<string> { "no such concept id" }
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostRestaurant(restaurant);
            });

            Assert.That(ex.Message, Is.StringContaining("Invalid Concept").IgnoreCase);
        }

        [Test]
        public void PutRestaurant_InvalidConceptId_ThrowsException()
        {
            var restaurant = _api.GetRestaurant(TestRestaurantId3);

            restaurant.Concepts = new List<string> {"no such concept id"};

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRestaurant(restaurant);
            });

            Assert.That(ex.Message, Is.StringContaining("Invalid Concept").IgnoreCase);
        }

        [Test]
        [Explicit("This test deletes a record, however we can't undelete a record. The Id may not be reused after deleting it.")]
        public void DeleteRetailLocation_ActiveLocation_CannotGetAndCannotList()
        {
            var deleteMeLocationId = $"{TestRestuarantIdPrefix}For_Deletion_1"; //increment the # after succesful test runs or delete all the data w/ an RQLCMD script.

            var retailLocation = new Restaurant
            {
                Id = deleteMeLocationId,
                LocationName = "Location For Deletion Unit Test",
                PrimaryPhoneNumber = "858-555-1212",
                FaxNumber = "555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Happy St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                }
            };

            var locationToDelete = _api.PostRestaurant(retailLocation);

            var locationAfterPost = _api.GetRestaurant(deleteMeLocationId);

            Assert.IsNotNull(locationAfterPost, "Should be able to GET it after POSTing it...");

            var allLocationsAfterPost = _api.ListRestaurants();

            Assert.That(allLocationsAfterPost.Select(a => a.Id).ToList(), Has.Some.Contains(locationToDelete.Id));

            _api.DeleteRestaurant(deleteMeLocationId);

            //should get error 404 - not found
            var ex = Assert.Throws<HttpRequestException>(() => _api.GetRetailLocation(deleteMeLocationId));

            Assert.That(ex.Message, Is.StringContaining("404"));

            //should not be listed either
            var allLocationsAfterDelete = _api.ListRestaurants();

            Assert.That(allLocationsAfterDelete.Select(a => a.Id).ToList(), Has.None.Contains(deleteMeLocationId));
        }


    }
}