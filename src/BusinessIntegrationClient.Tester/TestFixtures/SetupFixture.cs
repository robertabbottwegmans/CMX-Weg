using log4net.Config;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester.TestFixtures
{
    /// <summary>
    ///     Setup that occurs before any tests are invoked
    /// </summary>
    [SetUpFixture]
    public class SetupFixture
    {
        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure();
        }

        [TearDown]
        public void Teardown()
        {

        }
    }
}