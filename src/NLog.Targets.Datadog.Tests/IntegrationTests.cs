using System;
using System.Linq;
using FluentAssertions;
using NLog.Config;
using NLog.Target.Datadog;
using Xunit;

namespace NLog.Targets.Datadog.Tests
{
    public class TestData
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
    }

    public class IntegrationTests
    {
        private const string Greeting = "Hello from NLog.Target.DataDog!";

        [Fact(Skip = "Integration")]
        public void SimpleLogTest()
        {
            var logger = ConfigureLogger(LogLevel.Info);
            logger.Info(Greeting);
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
            
            LogManager.Configuration.AllTargets
                .OfType<DataDogTarget>()
                .First()
                .MaxRetries
                .Should().Be(666);

            var logger = LogManager.GetLogger("Example");

            logger.Trace(Greeting);
            logger.Debug(Greeting);
            logger.Info(Greeting);
            logger.Warn(Greeting);
            logger.Error(Greeting);
            logger.Fatal(Greeting);

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
                // IMPORTANT! replace "YOUR API KEY" with your DataDog API key
                ApiKey = "<YOUR API KEY>"
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