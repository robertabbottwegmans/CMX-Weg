using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business;
using RequirementsLive.Sdk.Api.Business.Model;
using RequirementsLive.Sdk.Api.Helper;

namespace BusinessIntegrationClient.Tester.TestFixtures
{
    [TestFixture]
    public class RqlBusinessApiTestFixtureBase
    {
        protected RqlCredential Credential { get; set; }

        protected IRqlBusinessApiClient ApiClient { get; set; }

        [TestFixtureSetUp]
        public void BaseFixtureSetup()
        {
            Credential = new RqlCredential
            {
                Site = ConfigurationManager.AppSettings["Site"],
                UserName = ConfigurationManager.AppSettings["UserName"],
                Password = ConfigurationManager.AppSettings["Password"]
            };

            Assert.That(Credential.Site, Is.Not.Null.And.Not.Empty, "The app.config doesn't have a value for key=Site");
            Assert.That(Credential.UserName, Is.Not.Null.And.Not.Empty,
                "The app.config doesn't have a value for key=UserName");
            Assert.That(Credential.Password, Is.Not.Null.And.Not.Empty,
                "The app.config doesn't have a value for key=Password");


            //will throw an exception if can't authenticate.
            ApiClient = ApiHelper.CreateAuthenticatedBusinessApiClient(Credential);
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
        public void BaseFixtureTeardown()
        {
        }

        /// <summary>
        ///     Formats a <see cref="ListObjectsResponse" /> to show returned properties and data returned by
        ///     <see cref="IRqlBusinessApiClient.ListObjects" />.
        ///     Useful for discoverying which fields are available for the purposes of building a SQL filter.
        /// </summary>
        /// <param name="request">the <see cref="ListObjects"/> request instance</param>
        /// <param name="response">the <see cref="ListObjectsResponse"/> response instance</param>
        /// <returns></returns>
        protected string FormatListObjectsResponse(ListObjects request, ListObjectsResponse response)
        {
            var message = new StringBuilder();
            message.AppendFormat("ListObjects for: {0}", request.AppName)
                .AppendFormat(" returned {0} Entity Properties and {1} row(s).",
                    response.Properties.Count, response.Items.Count)
                .AppendLine()
                .AppendLine("Properties:")
                .AppendFormat("\t{0}",
                    string.Join(", ", response.Properties.Select(p => p == "_StoreId" ? "StoreId" : p)))
                .AppendLine();

            //Note: any property name starting w/ underscore is a special property name, not actual usable in a filter.

            if (response.Items.Count > 0)
            {
                message.AppendLine("Properties and values:");
                response.Items.ForEach(item =>
                {
                    message.AppendFormat("\t{0}", string.Join(", ",
                        response.Properties.Select(propertyName => $"{propertyName} = {item[propertyName]}")));
                });
            }

            return message.ToString();
        }

        /// <summary>
        ///     This method will page through objects, providing a callback that receives the internal storeId.  This storeId may
        ///     be used to lookup an object, and updates performed on it.
        /// </summary>
        /// <param name="api">the API client instance</param>
        /// <param name="credential">the credential to use in case the API needs to be reauthenticated</param>
        /// <param name="appName">the name of the app objects are queried from</param>
        /// <param name="filter">a sql filter to limit the rows being processed</param>
        /// <param name="processObjectByStoreIdCallback">
        ///     a lamda that receives a storeId of the found Object.  Use the storeId to
        ///     get the object detail record via the API.
        /// </param>
        /// <returns>returns the # of objects iterated.</returns>
        protected static int IterateObjectsBasedOnFilter(IRqlBusinessApiClient api, RqlCredential credential,
            string appName, string filter,
            Action<string> processObjectByStoreIdCallback)
        {

            //Count the # of objects that match the filter
            //so we can page through the results.
            var numberOfObjects = api.CountObjects(new CountObjects
            {
                AppName = appName,
                Filter = filter
            }).Count;

            if (numberOfObjects == 0) return 0;

            //paging control variables:
            const int maxPageSize = 100; //could be bigger.
            var totalUpdated = 0;
            var pageIndex = 0;
            var pageSize = Math.Min(maxPageSize, numberOfObjects);

            bool moreToProcess;
            do
            {
                //reauthenticate periodically on long tasks
                ApiHelper.ReauthenticateIfNearingExpiration(api, credential);

                //get the current page full of users to update based on the filter
                var storeIds = api.ListObjects(new ListObjects
                    {
                        AppName = appName,
                        Filter = filter,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    })
                    .Items
                    .Select(item => item.StoreId)
                    .ToList();

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Debugger.IsAttached
                        ? 1
                        : Math.Min(Environment.ProcessorCount, 2)
                };
                Parallel.ForEach(storeIds, parallelOptions, (storeId, loopState) =>
                {
                    //if (loopState.ShouldExitCurrentIteration) return;

                    processObjectByStoreIdCallback(storeId);
                });

                totalUpdated += storeIds.Count;

                moreToProcess = storeIds.Count == pageSize &&
                                totalUpdated < numberOfObjects;

                pageIndex++;
            } while (moreToProcess);

            return numberOfObjects;
        }

    }
}