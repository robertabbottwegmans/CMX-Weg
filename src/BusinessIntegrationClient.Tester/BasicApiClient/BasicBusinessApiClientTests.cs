using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester.BasicApiClient
{
    [TestFixture]
    public class BasicBusinessApiClientTests
    {
        [Test]
        [Ignore("This isn't really implemented on the server side. code demo only")]
        public void GetJson_AutoAuthenticate_UsageExample()
        {
            var apiConfig = RqlApiConfiguration.FromAppConfig();

            var api = new BasicBusinessApiClient(apiConfig);

            var widget = api.GetJson<string>("Widgets/123");

            Console.WriteLine(widget);
        }

        [Test]
        [Ignore("This isn't really implemented on the server side. code demo only")]
        public void PostJson_ManuallyAuthenticate_UsageExample()
        {
            var apiConfig = RqlApiConfiguration.FromAppConfig();

            var api = new BasicBusinessApiClient(apiConfig);

            api.Authenticate();

            var widget = api.PostJson<string>("Widgets", new
                {
                    Id = "123",
                    Name = "Widget Name",
                    Type = "Type A Widget",
                    StringArray = new[] {"string 1", "string 2"},
                    ExtraInfo = new Dictionary<string, string>
                    {
                        {"Key1", "Value1"}
                    }
                }
            );

            Console.WriteLine(widget);
        }
    }
}