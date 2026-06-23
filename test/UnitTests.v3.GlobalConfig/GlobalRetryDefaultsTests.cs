using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3.GlobalConfig
{
    public class GlobalRetryDefaultsTests
    {
        private static readonly ConcurrentDictionary<int, int> theoryRetryCounts = new();

        private static int factRetryCount;
        private static int explicitRetryCount;
        private static Stopwatch configuredDelayStopwatch;
        private static Stopwatch explicitDelayStopwatch;

        [RetryFact]
        public void RetryFact_UsesConfiguredMaxRetries()
        {
            factRetryCount++;

            factRetryCount.Should().Be(4);
        }

        [RetryTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_UsesConfiguredMaxRetries(int id)
        {
            int actual = theoryRetryCounts.AddOrUpdate(id, 1, (_, retries) => retries + 1);

            actual.Should().Be(4);
        }

        [RetryFact(5)]
        public void RetryFact_ExplicitMaxRetries_OverrideConfiguredValue()
        {
            explicitRetryCount++;

            explicitRetryCount.Should().Be(5);
        }

        [RetryFact]
        public void RetryFact_UsesConfiguredDelayBetweenRetries()
        {
            configuredDelayStopwatch ??= Stopwatch.StartNew();

            configuredDelayStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(120);
        }

        [RetryFact(5, 100)]
        public void RetryFact_ExplicitDelayBetweenRetries_OverridesConfiguredValue()
        {
            explicitDelayStopwatch ??= Stopwatch.StartNew();

            explicitDelayStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(350);
        }
    }
}
