using System;
using System.Collections.Generic;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business.Dto;
using RequirementsLive.Sdk.Api.Business.Model;

namespace BusinessIntegrationClient.Tester.Api.Organizations
{
    [TestFixture]    
    public class OrganizationTests : RqlBusinessApiTestFixtureBase
    {
        private const string OrganizationAppName = "Core_Organization";

        private const string TestOrganizationId = "UnitTest_Organization_1";

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
        public void CountOrganizations(string filter)
        {
            var request = new CountObjects
            {
                AppName = OrganizationAppName,
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
        public void ListOrganizationsByFilter(string filter)
        {
            var request = new ListObjects
            {
                AppName = OrganizationAppName,
                Filter = filter
            };

            var response = ApiClient.ListObjects(request);

            var message = FormatListObjectsResponse(request, response);

            Console.WriteLine(message);
        }

        [Test]
        [Ignore("The Organization app in the back end hasn't implemented a handler for these requests yet. Don't run")]
        public void CanCreateOrUpdateOrganizationAndVerifyIt()
        {
            //A Organization can be a child of an Organization
            //ie: a Location has a parent Organization.
            //A Location may also not have a Parent Organization, and that's ok too.

            var organization = new Organization
            {
                OrganizationId = TestOrganizationId,
                OrganizationName = "Business API Unit Test Location",
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

            var storeId = ApiClient.PutOrganization(new PutOrganization {Organization = organization}).StoreId;

            Console.WriteLine("Organization {0} saved to StoreId {1}", organization.OrganizationId, storeId);

            //get one of them, assert they're the same as when passed in.

            var result = ApiClient.GetOrganization(new GetOrganization
            {
                Criteria = {StoreId = storeId}
            }).Organization;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StoreId, Is.StringStarting("str-"));
            Assert.That(result.OrganizationId, Is.EqualTo(organization.OrganizationId));
            Assert.That(result.OrganizationName, Is.EqualTo(organization.OrganizationName));
            Assert.That(result.BusinessType, Is.EqualTo(organization.BusinessType));
            Assert.That(result.Status, Is.EqualTo(organization.Status));
            Assert.That(result.PhoneNumber, Is.EqualTo(organization.PhoneNumber));
            Assert.That(result.FaxNumber, Is.EqualTo(organization.FaxNumber));

            Assert.That(result.PhysicalAddress, Is.Not.Null);
            Assert.That(result.PhysicalAddress.AddressLine1, Is.EqualTo(organization.PhysicalAddress.AddressLine1));
            Assert.That(result.PhysicalAddress.City, Is.EqualTo(organization.PhysicalAddress.City));
            Assert.That(result.PhysicalAddress.State, Is.EqualTo(organization.PhysicalAddress.State));
            Assert.That(result.PhysicalAddress.PostalCode, Is.EqualTo(organization.PhysicalAddress.PostalCode));
            Assert.That(result.PhysicalAddress.CountryCode, Is.EqualTo(organization.PhysicalAddress.CountryCode));
            Assert.That(result.PhysicalAddress.ExtraInformation["Notes"], Is.EqualTo(organization.PhysicalAddress.ExtraInformation["Notes"]));

            Assert.That(result.MailingAddress, Is.Not.Null);
            Assert.That(result.MailingAddress.AddressLine1, Is.EqualTo(organization.MailingAddress.AddressLine1));
            Assert.That(result.MailingAddress.City, Is.EqualTo(organization.MailingAddress.City));
            Assert.That(result.MailingAddress.State, Is.EqualTo(organization.MailingAddress.State));
            Assert.That(result.MailingAddress.PostalCode, Is.EqualTo(organization.MailingAddress.PostalCode));
            Assert.That(result.MailingAddress.CountryCode, Is.EqualTo(organization.MailingAddress.CountryCode));
            Assert.That(result.MailingAddress.ExtraInformation["Notes"], Is.EqualTo(organization.MailingAddress.ExtraInformation["Notes"]));

            Assert.That(result.ExtraInformation["MoreStuff"], Is.EqualTo(result.ExtraInformation["MoreStuff"]));
        }

        [Test]
        [Ignore("The Organization app in the back end hasn't implemented a handler for these requests yet. Don't run")]
        public void UpdateOrganizationsBasedOnAFilter()
        {
            var filter = $"Organization_ID = '{TestOrganizationId}' AND Status='Active'";

            var count = IterateObjectsBasedOnFilter(ApiClient, Credential, OrganizationAppName, filter,
                storeId =>
                {
                    //This callback receives our internal record id
                    //use that to get the desired Entity object, update it and put it back.

                    //look up the Asset
                    var organization = ApiClient.GetOrganization(new GetOrganization
                    {
                        Criteria = { StoreId = storeId }
                    }).Organization;


                    //TODO: update the Organization w/ meaningful updates

                    organization.Status = "Inactive";

                    //put the updated Location back
                    ApiClient.PutOrganization(new PutOrganization
                    {
                        Organization = organization
                    });
                });
        }
    }
}