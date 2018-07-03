using System;
using System.Linq;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business.Model;
using ServiceStack.ServiceClient.Web;

namespace BusinessIntegrationClient.Tester.Api
{
    [TestFixture]
    public class GeneralDiscoveryTests : RqlBusinessApiTestFixtureBase
    {

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

        [TestCase("Core_Asset_Tracking", Description = "This is the app associated with Assets & the PutAsset, GetAsset APIs")]
        [TestCase("Core_Organization", Description="This is the app associated with Organizations and the PutOrganization, GetOrganization APIs")]
        [TestCase("Core_Organization_Location", Description = "This is the app associated with Locations and the PutLocatio, GetLocation APIs")]

        [TestCase("User", Description ="This is app associated with Users and the PutUser, GetUser APIs.")]
        [TestCase("Core_Status", Description = "This is an app for lookup data regarding Status values for use in various places")]
        [TestCase("Core_Admin_Business_Type", Description = "This is an app for lookup data regarding BusinessType values used in Organization and Locations, and other places")]
        [TestCase("INVALID APP NAME",
            ExpectedException = typeof(WebServiceException),
            Reason = "invalid app name")]
        public void DisplayEntityColumnsAndData(string appName)
        {
            //Look at these results to see what
            //fields are available for use in a Filter.

            var request = new ListObjects
            {
                AppName = appName,
                PageIndex = 0,
                PageSize = 1
            };

            var response = ApiClient.ListObjects(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Properties, Is.Not.Null.And.Not.Empty);
            Assert.That(response.Items, Is.Not.Null);
            Assert.That(response.Items.Count, Is.LessThanOrEqualTo(request.PageSize));

            if (response.Items.Count > 0)
                Assert.That(response.Items.First().StoreId, Is.EqualTo(response.Items.First()["_StoreId"]));

            
            var message = FormatListObjectsResponse(request, response);

            Console.WriteLine(message);
            Console.WriteLine();
        }


        [TestCase("INVALID APP NAME", null,
            ExpectedException = typeof(WebServiceException),
            Reason = "invalid app name")]
        [TestCase("User", "Status = 'Active'")]
        [TestCase("User", "Status = 'Deleted'")]
        [TestCase("User", "Status = 'Missing Apostrophe",
            ExpectedException = typeof(WebServiceException),
            Reason = "invalid sql filter")]
        [TestCase("Core_Asset_Tracking", "Status = 'Active'")]
        [TestCase("Core_Asset_Tracking", "Status = 'Inactive'")]
        [TestCase("Core_Organization", null)]
        [TestCase("Core_Organization", "1 = 1")]
        [TestCase("Core_Organization", "1 = 2")]
        [TestCase("Core_Organization", "1 = 1; drop table dbo.faketable", 
            ExpectedException = typeof(WebServiceException),
            Reason = "invalid sql filter")]
        [TestCase("Core_Organization", "Status = 'Active'")]
        [TestCase("Core_Organization", "Status = 'Inactive'")]
        public void CountObjectsForAppAndFilter(string appName, string filter)
        {
            //
            //  Look at these results to get an idea of
            //  how many results there are for a given filter.
            //
            var request = new CountObjects
            {
                AppName = appName,
                Filter = filter
            };

            var response = ApiClient.CountObjects(request);

            Console.WriteLine("{0} {1} objects found for filter: {2}",
                response.Count, appName, filter ?? "<no filter>");
        }

    }
}