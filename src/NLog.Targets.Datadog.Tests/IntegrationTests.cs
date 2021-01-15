using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NLog.Config;
using NLog.Target.Datadog;
using Xunit;
using Xunit.Abstractions;

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
        private readonly ITestOutputHelper _testOutputHelper;

        public IntegrationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const string Greeting = "Hello from NLog.Target.DataDog!";

        [Fact(Skip = "Integration")]
        public void SimpleLogTest()
        {
            var logger = ConfigureLogger(LogLevel.Info);
            logger.Info(Greeting);
            LogManager.Flush();
        }

        [Fact(Skip = "Integration")]
        public void NonStop_TerminateMe_LogTest()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.Targets.Datadog.Tests.dll.config");
            var logger = LogManager.GetLogger("Example");

            const int logs = 10000;

            Task.Run(() =>
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                var size1 = Process.GetCurrentProcess().PrivateMemorySize64;
                _testOutputHelper.WriteLine("Memory before test: {0}", size1);

                for (int i = 0; i < logs; ++i)
                {
                    logger.Info($"{Greeting}: {i}");
                    Thread.Sleep(10);
                }

                while (true)
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                    var size2 = Process.GetCurrentProcess().PrivateMemorySize64;
                    Thread.Sleep(10000);
                    _testOutputHelper.WriteLine("Memory after test: {0}", size2);
                }
            }).GetAwaiter().GetResult();
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
                .Should().Be(1000000);

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
            var config = new LoggingConfiguration();

            var elasticTarget = new DataDogTarget
            {
                // IMPORTANT! replace "YOUR API KEY" with your DataDog API key
                ApiKey = "YOUR API KEY",
                MaxRetries = 10000,
                Service = Assembly.GetExecutingAssembly()?.GetName().Name,
                Source = Environment.MachineName
            };
            var rule = new LoggingRule("*", elasticTarget);
            rule.EnableLoggingForLevel(level);
            config.AddTarget(elasticTarget);
            config.LoggingRules.Add(rule);

            var consoleTarget = new ConsoleTarget("console");
            var consoleRule = new LoggingRule("*", consoleTarget);
            consoleRule.EnableLoggingForLevel(level);
            config.AddTarget(consoleTarget);
            config.LoggingRules.Add(consoleRule);

            var debugTarget = new DebugTarget("debug");
            var debugRule = new LoggingRule("*", debugTarget);
            debugRule.EnableLoggingForLevel(level);
            config.AddTarget(debugTarget);
            config.LoggingRules.Add(debugRule);

            LogManager.Configuration = config;

            var logger = LogManager.GetLogger("Example");
            return logger;
        }

    }
}