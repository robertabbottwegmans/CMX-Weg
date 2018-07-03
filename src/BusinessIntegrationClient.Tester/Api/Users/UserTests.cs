using System;
using System.Collections.Generic;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;
using RequirementsLive.Sdk.Api.Business.Dto;
using RequirementsLive.Sdk.Api.Business.Model;

namespace BusinessIntegrationClient.Tester.Api.Users
{
    [TestFixture]
    [Ignore("The back end User app hasn't implemented a handler for these API requests yet.  Calling PutUser will result in incomplete User records in the database that should be deleted later.")]
    public class UserTests : RqlBusinessApiTestFixtureBase
    {
        private const string UserAppName = "User";

        private const string TestUserName = "BusinessIntegration.Tester.UserTests";

        #region Test Fixture Setup / Teardown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EnsureTestUserExists();
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
            MarkTestUserDeleted();
        }

        #endregion

        /// <summary>
        /// Creates a test user if doesn't exist.
        /// </summary>
        private void EnsureTestUserExists()
        {
            //look up the user based on user name.
            var getUserResponse = ApiClient.GetUser(new GetUser
            {
                Criteria = new GetUserSearchCriteria
                {
                    UserName = TestUserName
                }
            });

            var user = getUserResponse.User;

            if (user == null)
            {
                //user doesn't exist yet, create it.

                var putUser = new PutUser
                {
                    //whenever you use PutUser always fill the entire User object out.
                    User = new User
                    {
                        UserName = TestUserName,
                        FirstName = "Unit",
                        LastName = "Test User",
                        Email = "UserTests.unit.test@" + Credential.Site,
                        Status = "Active",
                        LanguagePreference = "en-US",
                        //list of already existing organization Id this user will have access to
                        OrganizationIds = new List<string>
                        {
                            "Organization123"
                        },
                        //list of already existing location ids for this user.
                        LocationIds = new List<string>
                        {
                            //"Location123", "Location456"
                        },
                        //list of already existing profile ids for this user.
                        ProfileIds = new List<string>
                        {
                            //"Profile 123", "Profile 456"
                        },
                        //Keyvalue pair data of extra info:
                        ExtraInformation = new Dictionary<string, string>
                        {
                            {"Key1", "Value1"},
                            {"Stuff", "Good stuff"},
                            {"MoreStuff", "More Good stuff"}
                        },
                        //Set flag to trigger EULA when they login?
                        ForceEndUserLicenseAcceptance = false,
                        //Set flag to trigger email notification re: new user?
                        SendNewUserNotification = false
                    }
                };

                var response = ApiClient.PutUser(putUser);

                Console.WriteLine("PutUser returned external record id: {0}", response.StoreId);
            }
        }

        /// <summary>
        /// Updates the test user 
        /// </summary>
        private void MarkTestUserDeleted()
        {
            var user = ApiClient.GetUser(new GetUser
            {
                Criteria = new GetUserSearchCriteria
                {
                    UserName = TestUserName
                }
            }).User;

            if (user != null)
            {
                user.Status = "Deleted";

                ApiClient.PutUser(new PutUser
                {
                    User = user
                });
            }
        }


        [TestCase(null)]
        [TestCase("1=1")]
        [TestCase("Status = 'Active'")]
        public void CountUsers(string filter)
        {
            var request = new CountObjects
            {
                AppName = UserAppName,
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
        public void ListUsersByFilter(string filter)
        {
            var request = new ListObjects
            {
                AppName = UserAppName,
                Filter = filter
            };

            var response = ApiClient.ListObjects(request);

            var message = FormatListObjectsResponse(request, response);

            Console.WriteLine(message);
        }

        [Test]
        public void CanUpdateUserAndCanConfirmUpdate()
        {
            //GetUser only a proper result for a record created with PutUser.
            var user = ApiClient.GetUser(new GetUser
            {
                Criteria = new GetUserSearchCriteria
                {
                    StoreId = null,
                    UserName = TestUserName,
                    Email = null
                }
            }).User;

            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserName, Is.EqualTo(TestUserName));

            //alternate language preferences every time this test is run
            var oldLanguagePreference = user.LanguagePreference;

            var newLanguagePreference = oldLanguagePreference == "en-US"
                ? "fr-CA"
                : "en-US";

            user.LanguagePreference = newLanguagePreference;

            var userStoreId = ApiClient.PutUser(new PutUser
            {
                User = user
            }).StoreId;

            var userAfterUpdate = ApiClient.GetUser(new GetUser
            {
                Criteria = {StoreId = userStoreId}
            }).User;

            Assert.That(userAfterUpdate, Is.Not.Null);
            Assert.That(userAfterUpdate.LanguagePreference, Is.EqualTo(newLanguagePreference));
        }


        [Test]
        public void MarkUsersInactiveBasedOnAFilter()
        {
            var filter = $"User_Name = '{TestUserName}' AND Status='Active'";

            var count = IterateObjectsBasedOnFilter(ApiClient, Credential, UserAppName, filter,
                storeId =>
                {
                    //This callback receives our internal record id
                    //use that to get the desired Entity object, update it and put it back.

                    //look up the User
                    var user = ApiClient.GetUser(new GetUser
                    {
                        Criteria = {StoreId = storeId}
                    }).User;


                    //TODO: update the user w/ meaningful updates

                    user.Status = "Deleted";

                    //put the user back
                    ApiClient.PutUser(new PutUser
                    {
                        User = user
                    });
                });
        }

    }
}