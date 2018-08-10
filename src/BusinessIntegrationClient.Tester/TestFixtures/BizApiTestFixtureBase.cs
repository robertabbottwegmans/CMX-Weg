using System;
using System.Configuration;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester.TestFixtures
{
    [TestFixture]
    public class BizApiTestFixtureBase
    {
        protected RqlApiConfiguration _config;
        protected RestfulBusinessApiClient _api;

        /// <summary>
        ///     The Id of a Concept record - CREATE IT MANUALLY
        /// </summary>
        /// <remarks>
        ///     See Core App Master Queues->Concepts & create one w/ this Id.
        ///
        ///     Tests that rely on it should be marked Explicit.
        /// </remarks>
        protected const string UnitTestConceptId = "UnitTestConceptId";

        /// <summary>
        ///     The Id of a Concept record - CREATE IT MANUALLY
        /// </summary>
        /// <remarks>
        ///     See Core App Master Queues->Concepts & create one w/ this Id.
        ///
        ///     Tests that rely on it should be marked Explicit.
        /// </remarks>
        protected const string UnitTestConceptId2 = "UnitTestConceptId2";

        protected RqlApiConfiguration CreateConfiguration()
        {

            var site = ConfigurationManager.AppSettings["Site"];
            var userName = ConfigurationManager.AppSettings["UserName"];
            var password = ConfigurationManager.AppSettings["Password"];
            var useSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSsl"] ?? "True");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"] ?? "-1");

            Assert.That(site, Is.Not.Null.And.Not.Empty, "The app.config doesn't have a value for key=Site");
            Assert.That(userName, Is.Not.Null.And.Not.Empty,
                "The app.config doesn't have a value for key=UserName");
            Assert.That(password, Is.Not.Null.And.Not.Empty,
                "The app.config doesn't have a value for key=Password");

            return new RqlApiConfiguration
            {
                Site = site,
                UserName = userName,
                Password = password,
                UseSsl = useSsl,
                Port = port,
                //for local dev:
                //UseSsl = false,
                //Port = 8080
                UserAgent = "Unit Tests/1.0"
            };
        }

        [TestFixtureSetUp]
        public void BaseFixtureSetup()
        {
            _config = CreateConfiguration();

            _api = new RestfulBusinessApiClient(_config);

            _api.Authenticate();
        }

        [TestFixtureTearDown]
        public void BaseFixtureTearDown()
        {

        }
    }
}
