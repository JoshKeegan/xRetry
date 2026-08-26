using System;
using System.IO;
using FluentAssertions;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3
{
    public class RetryDefaultsTests
    {
        [Fact]
        public void Load_NoConfig_ReturnsDefaultValues()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path).Value;

            retryDefaults.Should().BeEquivalentTo(new RetryDefaults());
        }

        [Fact]
        public void Load_EmptyConfigFile_ReturnsError()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("");

            RetryDefaults.Load(tempDirectory.Path).Error.Should()
                .Contain("could not be read as valid JSON configuration");
        }

        [Fact]
        public void Load_MaxRetriesConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetries\": 5}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path).Value;

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().Be(5);
            retryDefaults.DelayBetweenRetriesMs.Should().BeNull();
        }

        [Fact]
        public void Load_DelayBetweenRetriesMsConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"delayBetweenRetriesMs\": 25}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path).Value;

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().BeNull();
            retryDefaults.DelayBetweenRetriesMs.Should().Be(25);
        }

        [Fact]
        public void Load_RetryUntaggedScenariosConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"retryUntaggedScenarios\": true}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path).Value;

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().BeNull();
            retryDefaults.DelayBetweenRetriesMs.Should().BeNull();
            retryDefaults.RetryUntaggedScenarios.Should().BeTrue();
        }

        [Fact]
        public void Load_SchemaConfigured_ReturnsConfiguredValues()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig(
                "{\"$schema\": \"https://raw.githubusercontent.com/JoshKeegan/xRetry/master/xretry.schema.json\", \"maxRetries\": 5}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path).Value;

            retryDefaults.Schema.Should().Be(
                "https://raw.githubusercontent.com/JoshKeegan/xRetry/master/xretry.schema.json");
            retryDefaults.MaxRetries.Should().Be(5);
        }

        [Fact]
        public void Load_InvalidMaxRetries_ReturnsError()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetries\": 0}");

            RetryDefaults.Load(tempDirectory.Path).Error.Should().Contain("maxRetries must be >= 1");
        }

        [Fact]
        public void Load_InvalidDelayBetweenRetriesMs_ReturnsError()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"delayBetweenRetriesMs\": -1}");

            RetryDefaults.Load(tempDirectory.Path).Error.Should().Contain("delayBetweenRetriesMs must be >= 0");
        }

        [Fact]
        public void Load_UnknownConfigKey_ReturnsError()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetry\": 5}");

            RetryDefaults.Load(tempDirectory.Path).Error.Should().Contain("maxRetry");
        }

        private sealed class TempDirectory : IDisposable
        {
            private TempDirectory(string path)
            {
                Path = path;
            }

            public string Path { get; }

            public static TempDirectory Create()
            {
                string path = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "xRetry-v3-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(path);
                return new TempDirectory(path);
            }

            public void WriteConfig(string json) =>
                File.WriteAllText(System.IO.Path.Combine(Path, RetryDefaults.FILE_NAME), json);

            public void Dispose()
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, recursive: true);
                }
            }
        }
    }
}
