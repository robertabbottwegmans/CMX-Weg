using System;
using System.Collections.Generic;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business.Dto;
using RequirementsLive.Sdk.Api.Business.Model;

namespace BusinessIntegrationClient.Tester.Api.Locations
{
    [TestFixture]    
    public class LocationTests : RqlBusinessApiTestFixtureBase
    {
        private const string LocationAppName = "Core_Organization_Location";

        private const string TestLocationId = "UnitTest_LocationId_1";

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {

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

        [TestCase(null)]
        [TestCase("1=1")]
        [TestCase("Status = 'Active'")]
        public void CountLocations(string filter)
        {
            var request = new CountObjects
            {
                AppName = LocationAppName,
                Filter = filter
            };

            var response = ApiClient.CountObjects(request);

            Console.WriteLine("{0} {1} objects found for filter: {2}",
                response.Count, request.AppName, filter ?? "<no filter>");
        }


        [TestCase(null)]
        [TestCase("1=1")]
        [TestCase("Status = 'Inactive'")]
        [TestCase("Status = 'Active'")]
        public void ListLocationsByFilter(string filter)
        {
            var request = new ListObjects
            {
                AppName = LocationAppName,
                Filter = filter
            };

            var response = ApiClient.ListObjects(request);

            var message = FormatListObjectsResponse(request, response);

            Console.WriteLine(message);
        }

        [Test]
        [Ignore("The Locations app in the back end hasn't implemented a handler for these requests yet. Don't run")]
        public void CanCreateOrUpdateLocationAndVerifyIt()
        {
            //A Location can be a child of an Organization
            //ie: a Location has a parent Organization.
            //A Location may also not have a Parent Organization, and that's ok too.

            var location = new Location
            {
                LocationId = TestLocationId,
                LocationName = "Business API Unit Test Location",
                BusinessType = "Supplier",
                ParentOrganization =
                    null, //<--no parent org for this test. otherwise must be reference to existing org.
                Status = "Active",
                PhysicalAddress = new Address
                {
                    AddressLine1 = "123 Main Street",
                    AddressLine2 = null,
                    City = "La Jolla",
                    State = "CA",
                    PostalCode = "92037",
                    CountryCode = "US",
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"Notes", "Gate Code is 1234"}
                    }
                },
                MailingAddress = new Address
                {
                    AddressLine1 = "PO Box 12345",
                    AddressLine2 = null,
                    City = "La Jolla",
                    State = "CA",
                    PostalCode = "92037",
                    CountryCode = "US",
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"Notes", "This is a PO Box"}
                    }
                },
                FaxNumber = "555-1212",
                PhoneNumber = "888-555-1212",


                ExtraInformation = new Dictionary<string, string>
                {
                    {"ExtraStuff1", "Stuff"},
                    {"MoreStuff", "GoodStuff"}
                }
            };

            var storeId = ApiClient.PutLocation(new PutLocation {Location = location}).StoreId;

            Console.WriteLine("Location {0} saved to StoreId {1}", location.LocationId, storeId);

            //get one of them, assert they're the same as when passed in.

            var result = ApiClient.GetLocation(new GetLocation
            {
                Criteria = {StoreId = storeId}
            }).Location;

            //GetLocation only a proper result for a record created with PutLocation.

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StoreId, Is.StringStarting("str-"));
            Assert.That(result.LocationId, Is.EqualTo(location.LocationId));
            Assert.That(result.LocationName, Is.EqualTo(location.LocationName));
            Assert.That(result.BusinessType, Is.EqualTo(location.BusinessType));
            Assert.That(result.Status, Is.EqualTo(location.Status));
            Assert.That(result.PhoneNumber, Is.EqualTo(location.PhoneNumber));
            Assert.That(result.FaxNumber, Is.EqualTo(location.FaxNumber));

            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.AddressLine1, Is.EqualTo(location.PhysicalAddress.AddressLine1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(location.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.State, Is.EqualTo(location.PhysicalAddress.State));
            Assert.That(result.PhysicalAddress.PostalCode, Is.EqualTo(location.PhysicalAddress.PostalCode));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(location.PhysicalAddress.CountryCode));
            Assert.That(result.PhysicalAddress.ExtraInformation["Notes"], Is.EqualTo(location.PhysicalAddress.ExtraInformation["Notes"]));

            Assert.That(result.MailingAddress, Is.Not.Null);
            Assert.That(result.MailingAddress.AddressLine1, Is.EqualTo(location.MailingAddress.AddressLine1));
            Assert.That(result.MailingAddress.City, Is.EqualTo(location.MailingAddress.City));
            Assert.That(result.MailingAddress.State, Is.EqualTo(location.MailingAddress.State));
            Assert.That(result.MailingAddress.PostalCode, Is.EqualTo(location.MailingAddress.PostalCode));
            Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(location.MailingAddress.CountryCode));
            Assert.That(result.MailingAddress.ExtraInformation["Notes"], Is.EqualTo(location.MailingAddress.ExtraInformation["Notes"]));

            Assert.That(result.ExtraInformation["MoreStuff"], Is.EqualTo(result.ExtraInformation["MoreStuff"]));
        }

        [Test]
        [Ignore("The Locations app in the back end hasn't implemented a handler for these requests yet. Don't run")]
        public void UpdateLocationsBasedOnAFilter()
        {
            var filter = $"Location_ID = '{TestLocationId}' AND Status='Active'";

            var count = IterateObjectsBasedOnFilter(ApiClient, Credential, LocationAppName, filter,
                storeId =>
                {
                    //This callback receives our internal record id
                    //use that to get the desired Entity object, update it and put it back.

                    //look up the Asset
                    var location = ApiClient.GetLocation(new GetLocation
                    {
                        Criteria = { StoreId = storeId }
                    }).Location;


                    //TODO: update the location w/ meaningful updates

                    location.Status = "Inactive";

                    //put the updated Location back
                    ApiClient.PutLocation(new PutLocation
                    {
                        Location = location
                    });
                });
        }
    }
}