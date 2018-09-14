using System;
using System.Collections.Generic;
using System.Xml;
using BusinessIntegrationClient.Dtos;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
{
    [TestFixture]
    public class DtoSerializationTests
    {
        private string ConvertJsonToXml(string json, string rootNodeName)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, rootNodeName);
            return doc.DocumentElement.OuterXml;
        }

        private string ConvertXmlToJson(string responseXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseXml);

            var json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, omitRootObject: true);
            //an empty Node returns the string "null" (w/out quotes)
            return (json ?? "null") == "null" ? string.Empty : json;
        }

        private void SerializeToJsonThenToXml(object @object)
        {
            var result = @object.ToJson();
            Console.WriteLine(result);

            var xmlFromJson2 = ConvertJsonToXml(result, "root");
            Console.WriteLine(xmlFromJson2);
        }

        private T SerializeToJsonThenToXmlThenToJsonThenTo<T>(T @object)
        {
            //serialize to json...
            var json = @object.ToJson();
            Console.WriteLine(json);

            //then to xml
            var xmlFromJson = ConvertJsonToXml(json, "root");
            Console.WriteLine(xmlFromJson);

            //then to json
            var jsonFromXml = ConvertXmlToJson(xmlFromJson);
            //then back to an instance of T
            return jsonFromXml.FromJson<T>();
        }

        [Test]
        public void User_ToJson_ThenToXml_ThenToJson_BackToUser()
        {
            var user = new User
            {
                UserName = "Jimmy.bobby.",
                Email = "email@jimmybobby.com",
                FirstName = "Jimmy",
                LastName = "Bobby",
                Title = "Sir",
                PrimaryPhoneNumber = "555-1212",
                MobilePhoneNumber = "858-555-1212",
                ContactTypes = new List<string>
                {
                    "ID",
                    "ID2"
                },

                AssociatedEntities = new List<EntityReference>
                {
                    new EntityReference {Id = "ID", EntityType = "Retail_Location"},
                    new EntityReference
                    {
                        Id = "ID",
                        EntityType = "Supplier",
                        Hierarchy = new Dictionary<string, string>
                        {
                            {"ExtraKey1", "ExtraValue1"},
                            {"ExtraKey2", "ExtraValue2"}
                        }
                    }
                },
                ExtraInformation = new Dictionary<string, string>
                {
                    {"ExtraKey1", "ExtraValue1"},
                    {"ExtraKey2", "ExtraValue2"}
                },
                Profiles = new List<string> {"Profile1", "Profile 2"},
                SendNewUserNotification = false,
                Hierarchies = new Hierarchies
                {
                    Hierarchy = 
                    {
                        new Dictionary<string, string> {{"Level1", "Level 1 value"}},
                        new Dictionary<string, string> {{"Key", "Value"}}
                    }
                }
            };


            var result = SerializeToJsonThenToXmlThenToJsonThenTo(user);

            Assert.AreEqual(user.UserName, result.UserName);            
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.MobilePhoneNumber, result.MobilePhoneNumber);
            Assert.AreEqual(user.PrimaryPhoneNumber, result.PrimaryPhoneNumber);
            Assert.AreEqual(user.Title, result.Title);
            Assert.IsNotNull(result.ContactTypes);
            Assert.AreEqual(user.ContactTypes.Count, result.ContactTypes.Count);
            Assert.AreEqual(user.ContactTypes[0], result.ContactTypes[0]);
            Assert.IsNotNull(result.AssociatedEntities);
            Assert.AreEqual(user.AssociatedEntities.Count, result.AssociatedEntities.Count);
            Assert.AreEqual(user.AssociatedEntities[0].Id, result.AssociatedEntities[0].Id);
            Assert.IsNotNull(result.ExtraInformation);
            Assert.AreEqual(user.ExtraInformation.Count, result.ExtraInformation.Count);
            Assert.AreEqual(user.ExtraInformation.Keys, result.ExtraInformation.Keys);
            CollectionAssert.AreEqual(user.ExtraInformation, result.ExtraInformation);
            Assert.AreEqual(user.Profiles[0], result.Profiles[0]);
            Assert.AreEqual(user.Profiles[1], result.Profiles[1]);
        }

        [Test]
        public void User_ToJson_ThenToXml()
        {
            var userName = "unit.test.user_1";

            var extraInfoKey2 = "StuffWithDifferentKey_1";
            var extraInfoValue2 = "Value 1";

            var user = new User
            {
                UserName = userName,
                Email = "noemail@compliancemetrix.com",
                PrimaryPhoneNumber = "858-555-1212",
                MobilePhoneNumber = "858-555-1212",
                Title = "Sir",
                FirstName = "Unit Test",
                LastName = "User Name",
                SendNewUserNotification = false,
                HasAllAccess = true,
                Profiles = new List<string> {"3rd Party Auditor"},
                ContactTypes = new List<string> {"Primary", "Emergency" },
                PhysicalAddress = new Address
                {
                    Address1 = "123 Main St",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US",
                },
                AssociatedEntities = new List<EntityReference>
                {
                    new EntityReference
                    {
                        Id = "Id 1234",
                        EntityType = "Restaurant"
                    }
                },
                AccessibleEntities = new List<EntityReference>
                {
                    new EntityReference
                    {
                        Id = "Id 1234",
                        EntityType = "Restaurant",
                        Hierarchy = new Dictionary<string, string>
                        {
                            {"Level1", "Level 1 Name"},
                            {"Level2", "Level 2 Name"},
                        }
                    }
                },

                ExtraInformation = new Dictionary<string, string>
                {
                    {"ExtraStuff", "Extra Stuff Value 1"},
                    {extraInfoKey2, extraInfoValue2}
                },
                Hierarchies = new Hierarchies
                {
                    Hierarchy = 
                    {
                        new Dictionary<string, string>
                        {
                            {"Level1-1", "Level 1.1 Value" },
                            {"Level1-2", "Level 1.2 Value" }
                        },
                        new Dictionary<string, string>
                        {
                            {"Level2-1", "Level 2.1 Value" }
                        },

                    }
                }
            };

            SerializeToJsonThenToXml(user);
        }


        [Test]
        public void User_WithEmptyExtraInformation_DeserializesFromJsonOk()
        {
            var json = @"{ ""UserName"": ""test"", ""ExtraInformation"": """", ""Hierarchies"": """" }";

            var result = json.FromJson<User>();

            Assert.IsNull(result.ExtraInformation);
            Assert.IsNull(result.Hierarchies);
        }

        [Test]
        public void UserSummary_ToJson_ThenToXml_ThenToJson_BackToUserSummary()
        {
            var user = new UserSummary
            {
                UserName = "jimmy.bobby",
                Email = "email@jimmybobby.com",
                FirstName = "Jimmy",
                LastName = "Bobby",                
                PrimaryPhoneNumber = "555-1212",
                MobilePhoneNumber = "858-555-1212",
                ProfileIds = "Profile1, Profile 2",
                Address1 = "123 Happy St",
                Address2 = null,
                City = "San Diego",
                StateProvince = "CA",
                ZipCode = "92109",
                Country = "United States"
            };

            var result = SerializeToJsonThenToXmlThenToJsonThenTo(user);
            
            Assert.AreEqual(user.UserName, result.UserName);
            Assert.AreEqual(user.Address1, result.Address1);
            Assert.AreEqual(user.Address2, result.Address2);
            Assert.AreEqual(user.City, result.City);
            Assert.AreEqual(user.Country, result.Country);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.MobilePhoneNumber, result.MobilePhoneNumber);
            Assert.AreEqual(user.PrimaryPhoneNumber, result.PrimaryPhoneNumber);
            Assert.AreEqual(user.ZipCode, result.ZipCode);
            Assert.AreEqual(user.ProfileIds, result.ProfileIds);
        }

        [Test]
        public void Asset_ToJson_ThenToXml()
        {
            var asset = new Asset
            {
                Id = "Asset 123",
                Type = "Test",
                SubType = "Unit Test",
                Description = "Asset Description",                
                ExtraInformation = new Dictionary<string, string>
                {
                    {"ExtraStuff", "ExtraValue"},
                    {"ExtraStuff2", "ExtraValue2"}
                }
            };

            var result = SerializeToJsonThenToXmlThenToJsonThenTo(asset);

            Assert.AreEqual(asset.Id, result.Id);
            CollectionAssert.AreEqual(asset.ExtraInformation, result.ExtraInformation);
        }

        [Test]
        public void RetailLocation_ToJson_ThenToXml()
        {
            var location = new RetailLocation
            {
                Id = "Location 123",
                LocationName = "Location 123 Name",
                FaxNumber = "555-1212",
                PrimaryPhoneNumber = "555-1212",
                PhysicalAddress = new Address
                {
                    Address1 = "123 Happy St",
                    Address2 = null,
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                    },
                MailingAddress = new Address
                {
                    Address1 = "Major Company Receiving Department",
                    Address2 = "PO BOX 1234567",
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                },
                Concepts = new List<string> {"123"},
                ExtraInformation = new Dictionary<string, string> {{"Department", "Meat"}}
            };

            SerializeToJsonThenToXml(location);
        }


        [Test]
        public void Restaurant_ToJson_ThenToXml()
        {
            var organization = new Restaurant
            {
                Id = "Location 123",
                LocationName = "Location 123 Name",
                PrimaryPhoneNumber = "555-1212",
                FaxNumber = "555-1212",
                Concepts = new List<string> {"Concept 123", "Concept 456"},
                PhysicalAddress = new Address
                {
                    Address1 = "123 Happy St",
                    Address2 = null,
                    City = "San Diego",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                },
                MailingAddress = new Address
                {
                    Address1 = "Major Company Receiving Department",
                    Address2 = "PO BOX 1234567",
                    StateProvinceCode = "CA",
                    ZipCode = "92109",
                    CountryCode = "US"
                },
                ExtraInformation = new Dictionary<string, string> {{"Department", "Meat"}}
            };

            SerializeToJsonThenToXml(organization);
        }


        [Test]
        public void ListOfUsersSummariesJson_ToList()
        {
            var json = @"[{
		""LastName"": ""Agri_User"",
		""FirstName"": ""Agri_User"",
		""UserName"": ""Agri_User"",
		""ProfileIds"": ""Program Admin,Program Admin2""
	}]";
            var users = JsonConvert.DeserializeObject<List<UserSummary>>(json);

            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users[0].UserName, Is.EqualTo("Agri_User"));
        }

        [Test]
        public void Sample()
        {
            var xml = @"
<Variable>
	<Request httpVerb=""GET"" resourceName=""widgets"">
		<!-- snipped -->		
	</Request>
	<Response>
		<HttpStatus code=""200"">OK</HttpStatus>
		<Body xmlns:json=""http://james.newtonking.com/projects/json"">
			<Id>123</Id>
			<Name>My name is Widget</Name>
			<Type>Type A Widget</Type>
			<StringArray json:Array=""true"">array item 1</StringArray>
			<StringArray json:Array=""true"">array item 2</StringArray>
			<ExtraInfo>
				<Key1>value 1</Key1>
				<Key2>value 1</Key2>
			</ExtraInfo>	
		</Body>
	</Response>	
</Variable>
";

            Console.WriteLine(ConvertXmlToJson(xml));


            var json = @"
{
	""Id"": ""123"",
	""Name"": ""Thing 1"",
	""Type"": ""Type A Widget"",
	""ExtraInfo"": {
		""Key1"": ""Value1"",
		""Key2"": ""Value2""
	}
}";
            Console.WriteLine(ConvertJsonToXml(json, "widget"));

        }

    }
}