using System;
using System.Linq;
using FluentAssertions;
using NLog.Config;
using NLog.Target.Datadog;
using Xunit;

namespace NLog.Targets.ElasticSearch.Tests
{
    public class TestData
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
    }

    public class IntegrationTests
    {
        [Fact(Skip = "Integration")]
        public void SimpleLogTest()
        {
            var logger = ConfigureLogger(LogLevel.Info);
            logger.Info("Hello elasticsearch");
            LogManager.Flush();
        }

        [Fact(Skip = "Integration")]
        public void ExceptionTest()
        {
            var logger = ConfigureLogger(LogLevel.Error);
            var exception = new ArgumentException("Some random error message");
            logger.Error(exception, "An exception occured");
            LogManager.Flush();
        }

        [Fact]
        public void StructuredData()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.Targets.Datadog.Tests.dll.config");
            GlobalDiagnosticsContext.Set("Version", "1.2.3-test");
            var logger = LogManager.GetLogger("Example");

            logger.Info("Here is some structured data {@person}", new TestData
            {
                Name = "Foo",
                Age = 20,
                Id = Guid.NewGuid()
            });

            LogManager.Flush();
        }

        [Fact]
        public void ReadFromConfigTest()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.Targets.Datadog.Tests.dll.config");
            
            var dataDogTarget = LogManager.Configuration.AllTargets.OfType<DataDogTarget>().First();
            dataDogTarget.MaxRetries.Should().Be(666);

            var logger = LogManager.GetLogger("Example");

            logger.Trace("Hello elasticsearch");
            logger.Debug("Hello elasticsearch");
            logger.Info("Hello elasticsearch");
            logger.Warn("Hello elasticsearch");
            logger.Error("Hello elasticsearch");
            logger.Fatal("Hello elasticsearch");

            LogManager.Flush();
        }

        [Fact]
        public void TestException()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.Targets.Datadog.Tests.dll.config");
            var logger = LogManager.GetLogger("Example");
            try
            {
                Foo();
            }
            catch (Exception e)
            {
                logger.Error(e, "Oops");
            }
            LogManager.Flush();
        }

        private static void Foo() => Bar();

        private static void Bar() => throw new Exception("Oops Exception");

        private static Logger ConfigureLogger(LogLevel level)
        {
            var elasticTarget = new DataDogTarget
            {
                ApiKey = "3f5d1caba8e3c8595c75e44e695f5470"
            };

            var rule = new LoggingRule("*", elasticTarget);
            rule.EnableLoggingForLevel(level);

            var config = new LoggingConfiguration();
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

            var logger = LogManager.GetLogger("Example");
            return logger;
        }

    }
}