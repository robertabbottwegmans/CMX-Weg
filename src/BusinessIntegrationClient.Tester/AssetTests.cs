using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using BusinessIntegrationClient.Dtos;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
{
    public class AssetTests : BizApiTestFixtureBase
    {
        private const int NumberOfTestAssets = 10;

        /// <summary>
        /// Use this prefix Id for all asset ids to make it easier to delete these records w/ a script.
        /// </summary>
        private const string TestAssetIdPrefix = "TestAsset_";
        private const string TestAssetId = "TestAsset_1";

        private void CreateTestAssetRecords()
        {
            var allAssetIds = _api.ListAllAssets()
                .Select(a => a.Id)
                .ToList();

            var assets = Enumerable.Range(1, NumberOfTestAssets).Select(i =>
            {
                var assetId = $"{TestAssetIdPrefix}{i}";

                var extraInfoKey2 = $"StuffWithDifferentKey_{i}";
                var extraInfoValue2 = $"Value {i}";
                var asset = new Asset
                {
                    Id = $"TestAsset_{i}",
                    Type = "Test",
                    SubType = "Unit Test",
                    Description = "Unit Test Asset",
                    ExtraInformation = new Dictionary<string, string>
                    {
                        {"ExtraStuff", $"Extra Stuff Value {i}"},
                        {extraInfoKey2, extraInfoValue2}
                    }
                };
                if (!allAssetIds.Contains(assetId))
                {
                    var result = _api.PostAsset(asset);

                    Assert.That(result.Id, Is.EqualTo(asset.Id));
                    Assert.That(result.Type, Is.EqualTo(asset.Type));
                    Assert.That(result.SubType, Is.EqualTo(asset.SubType));
                    Assert.That(result.Description, Is.EqualTo(asset.Description));
                    Assert.That(result.ExtraInformation, Is.Not.Null.And.Not.Empty);
                    Assert.That(result.ExtraInformation[extraInfoKey2], Is.EqualTo(extraInfoValue2));
                }

                return asset;
            }).ToList();


            Assert.That(assets.Count, Is.EqualTo(NumberOfTestAssets));
        }

        #region Test Fixture Setup / Teardown
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            CreateTestAssetRecords();
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
        public void ListAllAssets_WithFilter_GetsAllMatchingFilter()
        {
            var assets = _api.ListAllAssets(a => a.Id == TestAssetId);

            Assert.That(assets.Count, Is.EqualTo(1));
        }

        [Test]
        public void ListAssets_NoPaging_GetsAllAssets()
        {
            var assets = _api.ListAssets();

            Assert.That(assets, Is.Not.Null.And.Not.Empty);
            Assert.That(assets.Count, Is.GreaterThanOrEqualTo(NumberOfTestAssets));

            //The ExtraInformation is not filled out when listing all the Assets.
            Assert.That(assets[0].ExtraInformation, Is.Null);
            Assert.That(assets[1].ExtraInformation, Is.Null);
            Assert.That(assets[2].ExtraInformation, Is.Null);
        }

        [Test]
        public void ListAssets_PageSize1_Gets1Asset()
        {
            var assets = _api.ListAssets(pageSize: 1);

            Assert.That(assets, Is.Not.Null.And.Not.Empty);
            Assert.That(assets.Count, Is.EqualTo(1));
        }

        [Test]        
        public void GetAsset_ById_GetsAsset()
        {
            var assetId = TestAssetId;
            var asset = _api.GetAsset(assetId);

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Id, Is.EqualTo(assetId));
            Assert.That(asset.Description, Is.Not.Null.And.Not.Empty);
            Assert.That(asset.Type, Is.Not.Null.And.Not.Empty);
            Assert.That(asset.SubType, Is.Not.Null.And.Not.Empty);            
            Assert.That(asset.ExtraInformation, Is.Not.Null.And.Not.Empty);
        }

        [TestCase(null, "Test", "Missing Required Field Test", "Fake Description")]
        [TestCase("MissingRequiredFieldTest_1", null, "Missing Required Field Test", "Fake Description")]
        [TestCase("MissingRequiredFieldTest_2", "Fake Type", null, "Fake Description")]
        [TestCase("MissingRequiredFieldTest_3", "Fake Type", "Fake SubType", null)]
        [TestCase(null, null, null, null)]
        public void PostAsset_MissingRequiredFields_ThrowsException(string id, string type, string subType, string description)
        {
            if (id != null) id = id + TestAssetIdPrefix;
            var asset = new Asset
            {
                Id = id,
                Type = type,
                SubType = subType,
                Description = description,
                ExtraInformation = new Dictionary<string, string>
                {
                    {"Key1", "Value1"}
                }
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostAsset(asset); 
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        public void PostAsset_AssetHasAllNullProperties_ThrowsException()
        {
            var asset = new Asset();

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PostAsset(asset);
            });

            Assert.That(ex.Message, Is.StringContaining("Invalid Request").IgnoreCase);
        }

        [Test]
        public void PostAsset_AssetExists_ThrowsException()
        {
            var foundAsset = _api.GetAsset(TestAssetId);

            Assert.IsNotNull(foundAsset);

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                //insert records w/ POST, will throw if already exists.
                //Use PUT to update.
                _api.PostAsset(foundAsset);
            });

            Assert.That(ex.Message, Is.StringContaining("already exist").IgnoreCase);
        }

        [TestCase(null, "Missing Required Field Test", "Fake Description")]
        [TestCase("Fake Type", null, "Fake Description")]
        [TestCase("Fake Type", "Fake SubType", null)]
        [TestCase(null, null, null)]
        public void PutAsset_MissingRequiredFields_ThrowsException(string type, string subType, string description)
        {
            var asset = _api.GetAsset(TestAssetId);

            asset.Type = type;
            asset.SubType = subType;
            asset.Description = description;
            asset.ExtraInformation = null;

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutAsset(asset);
            });

            Assert.That(ex.Message, Is.StringContaining("not provided"));
        }

        [Test]
        public void PutAsset_RouteHasId_ButAssetIsEmpty_ThrowsException()
        {
            var asset = new Asset();

            var ex  = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutJson<Asset>("Assets/FakeAssetId", asset);
            });

            Assert.That(ex.Message, Is.StringContaining("Invalid Request").IgnoreCase);
        }


        [Test]
        public void PutAsset_NoSuchAsset_ThrowsException()
        {
            var asset = new Asset
            {
                Id = "No such Id asdpofk0-9i23-9ri-9i2-9 .adfl",
                Type = "Test",
                SubType = "Unit Test",
                Description = "Fake Description"
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutAsset(asset);
            });

            Assert.That(ex.Message, Is
                .StringContaining("not found").IgnoreCase.Or
                .StringContaining("not exist").IgnoreCase);//IIS vs. self hosting result
        }

        [Test]
        public void PutAsset_ExistingAsset_UpdatesAsset()
        {
            var asset = _api.GetAsset(TestAssetId);

            Assert.IsNotNull(asset);

            //toggle this value each time the test is run...
            var newValue = asset.Description == "Unit Test Asset"
                ? "Updated Unit Test Asset"
                : "Unit Test Asset";

            asset.Description = newValue;

            var result = _api.PutAsset(asset);

            Assert.That(result.Description, Is.EqualTo(newValue));

            var result2 = _api.GetAsset(TestAssetId);

            Assert.That(result2.Description, Is.EqualTo(newValue));
        }

        [Test]
        public void PutAsset_AssetHasAllNullProperties_ThrowsException()
        {
            var asset = new Asset
            {
                Id = null,
                Type = null,
                SubType = null,
                Description = null,
                ExtraInformation = null
            };

            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.PutAsset(asset);
            });

            Assert.That(ex.Message, Is.StringContaining("Not Allowed").IgnoreCase);
        }


        [Test]
        [Explicit("This test deletes a record, however we can't undelete a record. The Id may not be reused after deleting it.")]
        public void DeleteAsset_ActiveAsset_CannotGetAndCannotList()
        {
            var deleteMeAssetId = $"{TestAssetIdPrefix}For_Deletion_1"; //increment the # after succesful test runs

            var asset = new Asset
            {
                Id = deleteMeAssetId,
                Type = "Test",
                SubType = "Unit Test",
                Description = "Asset for DELETE test",
            };
            
            var assetToDelete = _api.PostAsset(asset);

            var assetAfterPost = _api.GetAsset(deleteMeAssetId);

            Assert.IsNotNull(assetAfterPost, "Should be able to GET an asset after POSTing it...");

            var allAssetsAfterPost = _api.ListAssets();

            Assert.That(allAssetsAfterPost.Select(a => a.Id).ToList(), Has.Some.Contains(assetToDelete.Id));

            _api.DeleteAsset(deleteMeAssetId);

            //should get error 404 - not found
            var ex = Assert.Throws<HttpRequestException>(() => _api.GetAsset(deleteMeAssetId));

            Assert.That(ex.Message, Is.StringContaining("404"));

            //should not be listed either
            var allAssetsAfterDelete = _api.ListAssets();

            Assert.That(allAssetsAfterDelete.Select(a => a.Id).ToList(), Has.None.Contains(deleteMeAssetId));
        }

    }
}
