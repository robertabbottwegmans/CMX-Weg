using log4net.Config;
using NUnit.Framework;

namespace BusinessIntegrationClient.Tester
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
            BasicConfigurator.Configure(
                new log4net.Appender.ConsoleAppender
                {
                    Layout = new log4net.Layout.SimpleLayout()
                });
        }

        [TearDown]
        public void Teardown()
        {

        }
    }
}