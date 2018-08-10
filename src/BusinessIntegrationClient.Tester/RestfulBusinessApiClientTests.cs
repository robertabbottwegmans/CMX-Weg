using System;
using System.Net.Http;
using BusinessIntegrationClient.Dtos;
using BusinessIntegrationClient.Tester.TestFixtures;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
{
    [TestFixture]
    public class RestfulBusinessApiClientTests : BizApiTestFixtureBase
    {
        
        [Test]
        public void Authenticate_InvalidCredentials_ThrowsHttpRequestException()
        {
            var config = CreateConfiguration();
            config.UserName = "bad.bad.ae23rascasedfa";

            var api = new RestfulBusinessApiClient(config);

            Assert.Throws<HttpRequestException>(() => api.Authenticate());
        }

        [Test]
        public void ListCountries_ListsCountries()
        {
            var result = _api.ListCountries();

            Assert.IsNotNull(result);
            //table may be empty...
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));
            
            foreach (var country in result)
            {
                Console.WriteLine("Country Code: {0}, Name: {1}", country.CountryCode, country.CountryName);

                Assert.That(country.CountryCode, Is.Not.Null.And.Not.Empty);
                Assert.That(country.CountryName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListCountries_PageSize1_Gets1()
        {
            var result = _api.ListCountries(pageSize: 1);

            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
            //may not be records in table

            if (result.Count > 0)
            {
                Assert.That(result[0].CountryCode, Is.Not.Null.And.Not.Empty);
                Assert.That(result[0].CountryName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListStates_ListsStates()
        {
            var states = _api.ListStates();

            Assert.That(states, Is.Not.Null.And.Not.Empty);

            foreach (var state in states)
            {
                Console.WriteLine("Country Code: {0}, State Code: {1}, State Name: {2}", state.CountryCode, state.StateProvinceCode, state.StateProvinceName);

                Assert.That(states[0].CountryCode, Is.Not.Null.And.Not.Empty);
                Assert.That(states[0].StateProvinceCode, Is.Not.Null.And.Not.Empty);
                Assert.That(states[0].StateProvinceName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListStates_PageSize1_Gets1()
        {
            var states = _api.ListStates(pageSize: 1);

            Assert.That(states.Count, Is.EqualTo(1));
            Assert.That(states[0].CountryCode, Is.Not.Null.And.Not.Empty);
            Assert.That(states[0].StateProvinceCode, Is.Not.Null.And.Not.Empty);
            Assert.That(states[0].StateProvinceName, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ListStatesByCountry_ListsStates()
        {
            var states = _api.ListStatesByCountry("US");

            foreach (var state in states)
            {
                Assert.That(state.CountryCode, Is.EqualTo("US"));

                Console.WriteLine("Country Code: {0}, State Code: {1}, State Name: {2}", state.CountryCode, state.StateProvinceCode, state.StateProvinceName);
            }
        }

        [Test]
        public void ListProfiles_ListsProfiles()
        {
            var result = _api.ListProfiles();

            Assert.That(result, Is.Not.Null.And.Not.Empty);

            foreach (var profile in result)
            {
                Console.WriteLine("Profile Id: {0}, Profile Name: {1}", profile.ProfileId, profile.ProfileName);

                Assert.That(profile.ProfileId, Is.Not.Null.And.Not.Empty);
                Assert.That(profile.ProfileName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListProfiles_PageSize1_Gets1()
        {
            var result = _api.ListProfiles(pageSize: 1);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ProfileId, Is.Not.Null.And.Not.Empty);
            Assert.That(result[0].ProfileName, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ListEntityTypes_ListsEntityTypes()
        {
            var result = _api.ListEntityTypes();

            Assert.That(result, Is.Not.Null.And.Not.Empty);

            foreach (var entityType in result)
            {
                Console.WriteLine("Entity Type Id: {0}, Entity Type Name: {1}", entityType.EntityTypeId, entityType.EntityTypeName);

                Assert.That(entityType.EntityTypeId, Is.Not.Null.And.Not.Empty);
                Assert.That(entityType.EntityTypeName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListEntityTypes_PageSize1_Returns1()
        {
            var result = _api.ListEntityTypes(pageSize: 1);

            Assert.That(result.Count, Is.EqualTo(1));

            Assert.That(result[0].EntityTypeId, Is.Not.Null.And.Not.Empty);
            Assert.That(result[0].EntityTypeName, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ListConcepts_ListsConcepts()
        {
            var result = _api.ListConcepts();

            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));

            foreach (var concept in result)
            {
                Console.WriteLine("Concept Id: {0}, Concept Name: {1}", concept.ConceptId, concept.ConceptName);

                Assert.That(concept.ConceptId, Is.Not.Null.And.Not.Empty);
                Assert.That(concept.ConceptName, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public void ListConcepts_PageSize1_Gets1()
        {
            var result = _api.ListConcepts(pageSize: 1);

            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
            //There may not be any rows in this table, expect 0 as a valid result.
        }

        [Test]
        public void ListContactTypes_ListsContactTypes()
        {
            var result = _api.ListContactTypes();

            Assert.That(result, Is.Not.Null.And.Not.Empty);

            foreach (var contactType in result)
            {
                Console.WriteLine("Code: {0}, Name: {1}", 
                    contactType.ContactTypeCode, contactType.ContactTypeName);

                Assert.That(contactType.ContactTypeCode, Is.Not.Null.And.Not.Empty);
                Assert.That(contactType.ContactTypeCode, Is.Not.Null.And.Not.Empty);
            }
        }

        [TestCase("Primary")]
        public void GetContactType_ByCode_NotImplemented(string contactTypeCode)
        {
            var ex = Assert.Throws<HttpRequestException>(() =>
            {
                _api.GetJson<ContactType>("ContactType/" + contactTypeCode);
            });

            Assert.That(ex.Message, Is.StringContaining("Not Implemented").IgnoreCase);
        }

        [Test]
        public void ListContactTypes_PageSize1_Gets1()
        {
            var result = _api.ListContactTypes(pageSize: 1);

            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));

            Assert.That(result[0].ContactTypeCode, Is.Not.Null.And.Not.Empty);
            Assert.That(result[0].ContactTypeCode, Is.Not.Null.And.Not.Empty);
        }


    }
}