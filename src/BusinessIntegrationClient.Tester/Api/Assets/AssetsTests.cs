using System;
using System.Collections.Generic;
using System.Linq;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business.Dto;
using RequirementsLive.Sdk.Api.Business.Model;

namespace BusinessIntegrationClient.Tester.Api.Assets
{
    [TestFixture]
    public class AssetsTests : RqlBusinessApiTestFixtureBase
    {
        private const string AssetAppName = "Core_Asset_Tracking";

        private const string UnitTestAssetIdPrefix = "UnitTest_";

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {

        }

        [SetUp]
        public void BaseSetup()
        {
        }

        [TearDown]
        public void BaseTearDown()
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
        public void CountAssets(string filter)
        {
            var request = new CountObjects
            {
                AppName = AssetAppName,
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
        public void ListAssetsByFilter(string filter)
        {
            var request = new ListObjects
            {
                AppName = AssetAppName,
                Filter = filter
            };

            var response = ApiClient.ListObjects(request);

            var message = FormatListObjectsResponse(request, response);

            Console.WriteLine(message);
        }

        [TestCase(20, Explicit = true)]
        public void CreateABunchOfAssets(int numberOfAssets)
        {
            var assets = Enumerable.Range(1, numberOfAssets)
                .Select(i =>
                    new Asset
                    {
                        AssetId = string.Format("{0}Asset_Id_{1}", UnitTestAssetIdPrefix, i),
                        Type = "Test",
                        Subtype = "Unit Test",
                        Description = string.Format("Asset # {0} for Unit Test", i),
                        Status = "Active",
                        ExtraInformation = new Dictionary<string, string>
                        {
                            {"SerialNumber", i.ToString()},
                            {"ExtraStuff1", "Stuff"},
                            {"MoreStuff", "GoodStuff"}
                        }
                    }
                ).ToList();

            foreach (var asset in assets)
            {
                var storeId = ApiClient.PutAsset(new PutAsset { Asset = asset }).StoreId;

                Console.WriteLine("AssetId {0} saved to StoreId {1}", asset.AssetId, storeId);
            }

            //get one of them, assert they're the same as when passed in.
            var example = assets.Skip(2).Take(1).Single();

            var result = ApiClient.GetAsset(new GetAsset
            {
                Criteria = {AssetId = example.AssetId}
            }).Asset;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StoreId, Is.StringStarting("str-"));
            Assert.That(result.Description, Is.EqualTo(example.Description));
            Assert.That(result.Type, Is.EqualTo(example.Type));
            Assert.That(result.Subtype, Is.EqualTo(example.Subtype));
            Assert.That(result.Status, Is.EqualTo(example.Status));
            Assert.That(result.ExtraInformation, Is.EqualTo(example.ExtraInformation));
        }

        [Test]
        public void CanUpdateAssetAndCanConfirmUpdate()
        {
            var firstAssetStoreId = ApiClient.ListObjects(new ListObjects
                {
                    AppName = AssetAppName,
                    Filter = "[Type] = 'Test' AND Subtype = 'Unit Test' AND Status = 'Active'",
                    PageSize = 1
                })
                .Items.Select(i => i.StoreId)
                .FirstOrDefault();

            Assert.IsNotNullOrEmpty(firstAssetStoreId, "Ensure that an Asset exists before calling this test again.");

            var asset = ApiClient.GetAsset(new GetAsset
            {
                Criteria = new GetAssetSearchCriteria
                {
                    StoreId = firstAssetStoreId,
                    AssetId = null
                }
            }).Asset;

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.StoreId, Is.EqualTo(firstAssetStoreId));
            Assert.That(asset.AssetId, Is.StringStarting(UnitTestAssetIdPrefix));


            //alternate language preferences every time this test is run
            var oldSubtype = asset.Subtype;

            var newSubtype = oldSubtype == "Unit Test"
                ? "Unit Test Updated"
                : "Unit Test";

            asset.Subtype = newSubtype;

            var assetStoreId = ApiClient.PutAsset(new PutAsset
            {
                Asset = asset
            }).StoreId;

            var assetAfterUpdate = ApiClient.GetAsset(new GetAsset
            {
                Criteria = { StoreId = assetStoreId }
            }).Asset;

            Assert.That(assetAfterUpdate, Is.Not.Null);
            Assert.That(assetAfterUpdate.Subtype, Is.EqualTo(newSubtype));
        }


        [Test]
        public void UpdateAssetsBasedOnAFilter()
        {
            var filter = "[Type] = 'Test' AND Subtype = 'Unit Test' AND Status = 'Active'";

            var count = IterateObjectsBasedOnFilter(ApiClient, Credential, AssetAppName, filter,
                storeId =>
                {
                    //This callback receives our internal record id
                    //use that to get the desired Entity object, update it and put it back.

                    //look up the Asset
                    var asset = ApiClient.GetAsset(new GetAsset
                    {
                        Criteria = { StoreId = storeId }
                    }).Asset;


                    //TODO: update the Asset w/ meaningful updates

                    asset.Status = "Inactive";

                    //put the updated Asset back
                    ApiClient.PutAsset(new PutAsset
                    {
                        Asset = asset
                    });
                });
        }
    }
}