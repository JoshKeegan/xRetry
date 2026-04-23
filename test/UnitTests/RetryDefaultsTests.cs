using System;
using System.IO;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests
{
    public class RetryDefaultsTests
    {
        [Fact]
        public void Load_NoConfig_ReturnsDefaultValues()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path);

            retryDefaults.Should().BeEquivalentTo(new RetryDefaults());
        }

        [Fact]
        public void Load_EmptyConfig_ReturnsDefaultValues()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path);

            retryDefaults.Should().BeEquivalentTo(new RetryDefaults());
        }

        [Fact]
        public void Load_EmptyConfigFile_ThrowsInvalidOperationException()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("");

            Action act = () => RetryDefaults.Load(tempDirectory.Path);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*could not be read as valid JSON configuration*");
        }

        [Fact]
        public void Load_MaxRetriesConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetries\": 5}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path);

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().Be(5);
            retryDefaults.DelayBetweenRetriesMs.Should().BeNull();
        }

        [Fact]
        public void Load_DelayBetweenRetriesMsConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"delayBetweenRetriesMs\": 25}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path);

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().BeNull();
            retryDefaults.DelayBetweenRetriesMs.Should().Be(25);
        }

        [Fact]
        public void Load_RetryUntaggedScenariosConfigured_ReturnsConfiguredValue()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"retryUntaggedScenarios\": true}");

            RetryDefaults retryDefaults = RetryDefaults.Load(tempDirectory.Path);

            retryDefaults.Should().NotBeNull();
            retryDefaults.MaxRetries.Should().BeNull();
            retryDefaults.DelayBetweenRetriesMs.Should().BeNull();
            retryDefaults.RetryUntaggedScenarios.Should().BeTrue();
        }

        [Fact]
        public void Load_InvalidMaxRetries_ThrowsInvalidOperationException()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetries\": 0}");

            Action act = () => RetryDefaults.Load(tempDirectory.Path);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*maxRetries must be >= 1*");
        }

        [Fact]
        public void Load_UnknownConfigKey_ThrowsInvalidOperationException()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetry\": 5}");

            Action act = () => RetryDefaults.Load(tempDirectory.Path);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*maxRetry*");
        }

        [Fact]
        public void Load_ConfigIsNotObject_ThrowsInvalidOperationException()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("[]");

            Action act = () => RetryDefaults.Load(tempDirectory.Path);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Load_ConfigInParentDirectory_ReturnsDefaultValues()
        {
            using TempDirectory tempDirectory = TempDirectory.Create();
            tempDirectory.WriteConfig("{\"maxRetries\": 5, \"delayBetweenRetriesMs\": 25}");
            string childDirectory = Path.Combine(tempDirectory.Path, "child");
            Directory.CreateDirectory(childDirectory);

            RetryDefaults retryDefaults = RetryDefaults.Load(childDirectory);

            retryDefaults.Should().BeEquivalentTo(new RetryDefaults());
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
                    "xRetry-" + Guid.NewGuid().ToString("N"));
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
