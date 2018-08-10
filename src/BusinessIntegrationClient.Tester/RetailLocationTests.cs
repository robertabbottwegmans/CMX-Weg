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
    public class RetailLocationTests : BizApiTestFixtureBase
    {
        private const int NumberOfTestRetailLocations = 3;

        /// <summary>
        /// Use this prefix so scripts can more easily delete related records.
        /// </summary>
        private const string TestLocationIdPrefix = "TestRetailLocation_";
        private const string TestLocationId = "TestRetailLocation_1";
        private const string TestLocationId2 = "TestRetailLocation_2";


        /// <summary>
        ///     The Id of a Concept record - CREATE IT MANUALLY
        /// </summary>
        /// <remarks>
        ///     See Core App Master Queues->Concepts & create one w/ this Id.
        ///
        ///     Tests that rely on it should be marked Explicit.
        /// </remarks>
        private const string UnitTestConceptId = "UnitTestConceptId";

        private void CreateTestRetailLocations()
        {
            var allRetailLocationIds = _api.ListRetailLocations()
                .Select(a => a.Id)
                .ToList();

            var retailLocations = Enumerable.Range(1, NumberOfTestRetailLocations).Select(i =>
            {
                var retailLocationId = $"{TestLocationIdPrefix}{i}";

                var extraInfoKey2 = $"StuffWithDifferentKey_{i}";
                var extraInfoValue2 = $"Value {i}";
                var location = new RetailLocation
                {
                    Id = retailLocationId,
                    LocationName = $"Test Location {i}",
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
                        CountryCode = "US",
                    },
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"ExtraStuff", $"Extra Stuff Value {i}"},
                        {extraInfoKey2, extraInfoValue2}
                    }
                };
                if (!allRetailLocationIds.Contains(retailLocationId))
                {
                    var result = _api.PostRetailLocation(location);
                  //  var result = _api.PutRetailLocation(location);

                    Assert.That(result.Id, Is.EqualTo(location.Id));
                    Assert.That(result.LocationName , Is.EqualTo(location.LocationName));
                    Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(location.PrimaryPhoneNumber));
                    Assert.That(result.FaxNumber, Is.EqualTo(location.FaxNumber));
                    Assert.That(result.PhysicalAddress, Is.Not.Null);
                    Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(location.PhysicalAddress.Address1));
                    Assert.That(result.PhysicalAddress.Address2, Is.Null.Or.Empty);
                    Assert.That(result.PhysicalAddress.City, Is.EqualTo(location.PhysicalAddress.City));
                    Assert.That(result.PhysicalAddress.ZipCode, Is.EqualTo(location.PhysicalAddress.ZipCode));
                    Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(location.PhysicalAddress.CountryCode));
                    Assert.That(result.MailingAddress, Is.Not.Null);
                    Assert.That(result.MailingAddress.Address1, Is.EqualTo(location.MailingAddress.Address1));
                    Assert.That(result.MailingAddress.Address2, Is.Null.Or.Empty);
                    Assert.That(result.MailingAddress.Address3, Is.Null.Or.Empty);
                    Assert.That(result.MailingAddress.City, Is.EqualTo(location.MailingAddress.City));
                    Assert.That(result.MailingAddress.StateProvinceCode, Is.EqualTo(location.MailingAddress.StateProvinceCode));
                    Assert.That(result.MailingAddress.ZipCode, Is.EqualTo(location.MailingAddress.ZipCode));
                    Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(location.MailingAddress.CountryCode));

                    
                    Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.ExtraInformation[extraInfoKey2], Is.EqualTo(extraInfoValue2));
                }

                return location;
            }).ToList();


            Assert.That(retailLocations.Count, Is.EqualTo(NumberOfTestRetailLocations));
        }

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            CreateTestRetailLocations();
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
        public void ListRetailLocations_NoPaging_ListsAll()
        {
            var result = _api.ListRetailLocations();
            
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(NumberOfTestRetailLocations));
        }

        [Test]
        public void ListRetailLocations_PageSize1_Gets1()
        {
            var result = _api.ListRetailLocations(pageSize: 1);

            Assert.That(result.Count, Is.EqualTo(1));
        }


        [Test]
        public void GetRetailLocation_ById_GetsIt()
        {
            var result = _api.GetRetailLocation(TestLocationId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(TestLocationId));
            Assert.That(result.LocationName, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PrimaryPhoneNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.FaxNumber, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.Address1, Is.Not.Null.And.Not.Empty);
            Assert.That(result.PhysicalAddress.Address2, Is.Not.Null.Or.Empty);
            Assert.That(result.PhysicalAddress.Address3, Is.Not.Null.Or.Empty);
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
        }

        [Test]
        public void GetRetailLocation_NoSuchId_ThrowsException()
        {
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.GetRetailLocation("This Id doesn't exist");
            });

            Assert.That(ex.Message, Is.StringContaining("not exist").IgnoreCase);
        }

        [Test]
        public void PostRetailLocation_AlreadyExists_ThrowsException()
        {
            var retailLocation = _api.GetRetailLocation(TestLocationId);

            Assert.IsNotNull(retailLocation);

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                //insert records w/ POST, will throw if already exists.
                //Use PUT to update.
                _api.PostRetailLocation(retailLocation);
            });

            Assert.That(ex.Message, Is.StringContaining("already exists"));
        }

        [TestCase("USSA!", "US")]
        [TestCase("US", "USSA!")]
        public void PostRetailLocation_InvalidCountryCode_ThrowsException(string physicalAddressCountryCode, string mailingAddressCountryCode)
        {
            var location = new RetailLocation
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
                _api.PostRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not a valid country code"));
        }

        [TestCase("CALI!", "CA")]
        [TestCase("CA", "CALI!")]
        public void PostRetailLocation_InvalidStateProvinceCode_ThrowsException(string physicalAddressStateProvinceCode, string mailingAddressStateProvinceCode)
        {
            var location = new RetailLocation
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
                    CountryCode = "US",
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
                _api.PostRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not a valid StateProvince"));
        }

        [TestCase(null, "Test Name", "123 Main St.", "San Diego", "US")]
        [TestCase("MissingRequiredField_Test_1", null, "123 Main St.", "San Diego", "US")]
        [TestCase("MissingRequiredField_Test_2", "Test", null, "San Diego", "US")]
        [TestCase("MissingRequiredField_Test_2", "Test", "123 Main St.", null, "US")]
        [TestCase("MissingRequiredField_Test_2", "Test", "123 Main St.", "San Diego", null)]
        [TestCase(null, null, null, null, null)]
        public void PostRetailLocation_WithMissingRequiredFields_ThrowsException(string id, string name, string address1,
            string city, string countryCode)
        {
            var location = new RetailLocation
            {
                Id = id,
                LocationName = name,
                FaxNumber = null,
                PrimaryPhoneNumber = null,
                ExtraInformation = null,
                MailingAddress = null,
                PhysicalAddress = new Address
                {
                    Address1 = address1,
                    City = city,
                    CountryCode = countryCode
                }
            };

            
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }
        
        [Test]
        public void PostRetailLocation_NoPhysicalAddress_ThrowsException()
        {
            var location = new RetailLocation
            {
                Id = "NoPhysicalAddressTest",
                LocationName = "No physical address test",
                PrimaryPhoneNumber = "858-555-1212",
                FaxNumber = "555-1212",
                ExtraInformation = new Dictionary<string, string>
                {
                    {"Lots", "OfStuff" }
                },
                MailingAddress = new Address
                {
                    Address1 = "PO Box 12345",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    CountryCode = "US"
                },
                PhysicalAddress = null //<-- this is required actually.
            };

            
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        public void PostRetailLocation_InvalidConceptId_ThrowsException()
        {
            var location = new RetailLocation
            {
                Id = $"{TestLocationIdPrefix}WithInvalidConceptIdTest",
                LocationName = "No physical address test",
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
                Concepts = new List<string> {"no such concept id"}
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("Invalid Concept").IgnoreCase);
        }

        [Test]
        [Explicit("This test should be run after a Concept w/ Id UnitTestConceptId has been manually created")]
        public void PostRetailLocation_ValidConceptId_Works()
        {
            var location = new RetailLocation
            {
                Id = $"{TestLocationIdPrefix}WithValidConceptIdTest",
                LocationName = "No physical address test",
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
                Concepts = new List<string> {UnitTestConceptId}
            };
            
            var result = _api.PostRetailLocation(location);

            Assert.That(result.Concepts, Is.Not.Null.And.Empty);
            Assert.That(result.Concepts[0], Is.EqualTo(location.Concepts[0]));

        }

        [Test]
        [Explicit("This test should be run after a Concept w/ Id UnitTestConceptId has been manually created")]
        public void PutRetailLocation_ValidConceptId_Works()
        {
            var location = _api.GetRetailLocation(TestLocationId2);

            location.Concepts = new List<string> {UnitTestConceptId};

            var result = _api.PutRetailLocation(location);

            Assert.That(result.Concepts, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Concepts[0], Is.EqualTo(location.Concepts[0]));
        }

        [Test]
        [Explicit("This test should be run after a Concept w/ Id UnitTestConceptId and UnitTestConceptId2 has been manually created")]
        public void PutRetailLocation_TwoValidConceptIds_Works()
        {
            var location = _api.GetRetailLocation(TestLocationId2);

            location.Concepts = new List<string> { UnitTestConceptId, UnitTestConceptId2 };

            var result = _api.PutRetailLocation(location);

            Assert.That(result.Concepts, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Concepts[0], Is.EqualTo(location.Concepts[0]));
            Assert.That(result.Concepts[1], Is.EqualTo(location.Concepts[1]));
        }


        [Test]
        public void PutRetailLocation_NoSuchId_ThrowsException()
        {
            var location = new RetailLocation
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
                _api.PutRetailLocation(location); 
            });

            Assert.That(ex.Message, Is.StringContaining("does not exist"));
        }

        [Test]
        public void PutRetailLocation_AllMembersNull_ThrowsException()
        {
            var location = new RetailLocation();

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRetailLocation(location); 
            });

            Assert.That(ex.Message, Is.StringContaining("Not Allowed").IgnoreCase);
        }

        [Test]
        public void PutRetailLocation_ExistingAsset_UpdatesAsset()
        {
            var retailLocation = _api.GetRetailLocation(TestLocationId);

            Assert.IsNotNull(retailLocation);

            //toggle this value each time the test is run...
            var newValue = retailLocation.PrimaryPhoneNumber == "858-555-1212"
                ? "+1-858-555-1212"
                : "858-555-1212";

            retailLocation.PrimaryPhoneNumber = newValue;

            var result = _api.PutRetailLocation(retailLocation);

            Assert.That(result.PrimaryPhoneNumber, Is.EqualTo(newValue));

            var result2 = _api.GetRetailLocation(TestLocationId);

            Assert.That(result2.PrimaryPhoneNumber, Is.EqualTo(newValue));
        }

        [Test]
        public void PutRetailLocation_ClearMailingAddress_RemovesMailingAddress()
        {
            var location = _api.GetRetailLocation(TestLocationId2);

            Assert.That(location.MailingAddress, Is.Not.Null);
            location.MailingAddress = null;

            var result = _api.PutRetailLocation(location);
            try
            {
                Assert.That(result.MailingAddress, Is.Null);

                var locationAfterPut = _api.GetRetailLocation(TestLocationId2);

                Assert.That(locationAfterPut.MailingAddress, Is.Null);
            }
            finally
            {
                //give it an mailing address again...
                location.MailingAddress = new Address
                {
                    Address1 = "PO BOX 123456",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                };
                _api.PutRetailLocation(location);
            }
        }

        [TestCase(null, "123 Main St.", "San Diego", "US")]
        [TestCase("Test", null, "San Diego", "US")]
        [TestCase("Test", "123 Main St.", null, "US")]
        [TestCase("Test", "123 Main St.", "San Diego", null)]
        [TestCase(null, null, null, null)]
        public void PutRetailLocation_UpdateExistingWithMissingRequiredFields_ThrowsException(string name, string address1,
            string city, string countryCode)
        {
            var location = _api.GetRetailLocation(TestLocationId);

            location.LocationName = name;
            location.PhysicalAddress.Address1 = address1;
            location.PhysicalAddress.City = city;
            location.PhysicalAddress.CountryCode = countryCode;

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRetailLocation(location); 
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        public void PutRestaurant_AddressInCountryWithoutStateCodesInDB_IsOk()
        {
            var location = _api.GetRetailLocation(TestLocationId2);

            Assert.That(location.PhysicalAddress, Is.Not.Null);
            location.PhysicalAddress.Address1 = "123 Somewhere st.";
            location.PhysicalAddress.City = "Buenas Aires";
            location.PhysicalAddress.StateProvinceCode = "BA";//<-- we don't have Argentine pronvinces in our DB
            location.PhysicalAddress.CountryCode = "AR";

            location.MailingAddress = new Address
            {
                Address1 = "123 Somewhere st.",
                City = "Buenas Aires",
                StateProvinceCode = "BA",
                CountryCode = "AR"
            };
            

            var result = _api.PutRetailLocation(location);

            Assert.That(result.PhysicalAddress.Address1, Is.EqualTo(location.PhysicalAddress.Address1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(location.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.StateProvinceCode, Is.EqualTo(location.PhysicalAddress.StateProvinceCode));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(location.PhysicalAddress.CountryCode));

            Assert.That(result.MailingAddress, Is.Not.Null);
            Assert.That(result.MailingAddress.Address1, Is.EqualTo(location.MailingAddress.Address1));
            Assert.That(result.MailingAddress.City, Is.EqualTo(location.MailingAddress.City));
            Assert.That(result.MailingAddress.StateProvinceCode, Is.EqualTo(location.MailingAddress.StateProvinceCode));
            Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(location.MailingAddress.CountryCode));
        }


        [Test]
        public void PutRetailLocation_UpdateExisting_NoPhysicalAddress_ThrowsException()
        {
            var location = _api.GetRetailLocation(TestLocationId);

            Assert.That(location.PhysicalAddress, Is.Not.Null);

            location.PhysicalAddress = null;
            
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutRetailLocation(location);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        [Explicit("This test deletes a record, however we can't undelete a record. The Id may not be reused after deleting it.")]
        public void DeleteRetailLocation_ActiveLocation_CannotGetAndCannotList()
        {
            var deleteMeLocationId = $"{TestLocationIdPrefix}For_Deletion_3"; //increment the # after succesful test runs or delete all data w/ a batch file and RQLCMD commands.(see QueryStores,DelStores).

            var retailLocation = new RetailLocation
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

            var locationToDelete = _api.PostRetailLocation(retailLocation);

            var locationAfterPost = _api.GetRetailLocation(deleteMeLocationId);

            Assert.IsNotNull(locationAfterPost, "Should be able to GET it after POSTing it...");

            var allLocationsAfterPost = _api.ListRetailLocations();

            Assert.That(allLocationsAfterPost.Select(a => a.Id).ToList(), Has.Some.Contains(locationToDelete.Id));

            _api.DeleteRetailLocation(deleteMeLocationId);

            //should get error 404 - not found
            var ex = Assert.Throws<HttpRequestException>(() => _api.GetRetailLocation(deleteMeLocationId));

            Assert.That(ex.Message, Is.StringContaining("404"));

            //should not be listed either
            var allLocationsAfterDelete = _api.ListRetailLocations();

            Assert.That(allLocationsAfterDelete.Select(a => a.Id).ToList(), Has.None.Contains(deleteMeLocationId));
        }

    }
}