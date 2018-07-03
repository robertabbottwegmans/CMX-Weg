using System;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business;
using RequirementsLive.Sdk.Api.Helper;
using ServiceStack.ServiceClient.Web;

namespace BusinessIntegrationClient.Tester.Api
{
    /// <summary>
    /// Test Fixture Containing various ways to instantiate the API client and creation behaviors.
    /// </summary>
    [TestFixture]
    public class ApiClientCreationTests : RqlBusinessApiTestFixtureBase
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

        [Test]
        public void CanCreateApiUsingConstructor()
        {
            var api = new RqlBusinessApiClient(Credential.Site);

            api.Authenticate(Credential.UserName, Credential.Password);

            Assert.Pass();//would have gotten exception w/ bad credentials
        }

        [Test]
        public void CanCreateApiUsingConstructor2()
        {
            var config = new RqlBusinessApiClientConfiguration
            {
                Site = Credential.Site,
                RequestTimeout = TimeSpan.FromSeconds(30)
            };
            var api = new RqlBusinessApiClient(config);

            api.Authenticate(Credential.UserName, Credential.Password);

            Assert.Pass(); //no exception on authenticate, it's ok.
        }

        [Test]
        public void Authenticate_BadCredentials_ThrowsWebServiceException()
        {
            var api = new RqlBusinessApiClient(Credential.Site);

            string badUserName= "bad.bad.=0=-03==231";
            var badPassword = "bad/bad/asdfasd=-f009-";

            Assert.Throws<WebServiceException>(() => api.Authenticate(badUserName, badPassword));
        }

        [Test]
        public void CanCreate_Using_ApiHelper_GoodCredential_IsOk()
        {
            var api = ApiHelper.CreateAuthenticatedBusinessApiClient(Credential);

            Assert.That(api, Is.Not.Null);
        }

        [Test]
        public void CanCreate_Using_ApiHelper_BadCredential_ThrowsWebServiceException()
        {
            var badCredential = new RqlCredential
            {
                Site = Credential.Site,
                UserName = "bad.user.name.xxadfasdf31",
                Password = "bad.password.adf23r2r-0"
            };
            Assert.Throws<WebServiceException>(() => ApiHelper.CreateAuthenticatedBusinessApiClient(badCredential));
        }

        [Test]
        public void CanCreate_Using_AuthenticatedApiClientFactory_GoodCredential_ReturnsNotNull()
        {
            var api = new AuthenticatedApiClientFactory()
                .CreateBusinessApiClient(Credential);

            Assert.That(api, Is.Not.Null);
            Assert.That(api.AuthenticatedUserName, Is.EqualTo(Credential.UserName));

        }

        [Test]
        public void CanCreate_Using_AuthenticatedApiClientFactory_BadCredential_ReturnsNull()
        {
            var badCredential = new RqlCredential(Credential)
            {
                UserName = "bad.bad.ba.d0920=-0w=-foas[f",
                Password = "bad bad badadf12=3-=asdfas=0-=a"
            };

            var api = new AuthenticatedApiClientFactory()
                .CreateBusinessApiClient(badCredential);

            Assert.That(api, Is.Null);
        }

        [Test]
        public void CanReauthenticateDuringLongRunningOperations()
        {
            //When the API client is authenticated, the authentication ticket 
            //is valid for one hour. 
            //If you are performing a long running task, you need to reauthenticate periodically
            //or else you'll get authentication exceptions in the middle of the task.
            //

            //Call this periodically during long running tasks:
            //  recommended at the beginning each a loop iteration when processing long lists.
            
            
            ApiHelper.ReauthenticateIfNearingExpiration(ApiClient, Credential);

            Assert.That(ApiClient.AuthenticationTicketIssued, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromMinutes(5)));
        }


    }
}