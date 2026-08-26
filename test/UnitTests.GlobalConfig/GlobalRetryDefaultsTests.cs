using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests.GlobalConfig
{
    public class GlobalRetryDefaultsTests
    {
        private static readonly ConcurrentDictionary<int, int> theoryRetryCounts = new ConcurrentDictionary<int, int>();
        private static readonly ConcurrentDictionary<int, int> explicitTheoryRetryCounts =
            new ConcurrentDictionary<int, int>();
        private static readonly ConcurrentDictionary<int, Stopwatch> theoryConfiguredDelayStopwatches =
            new ConcurrentDictionary<int, Stopwatch>();
        private static readonly ConcurrentDictionary<int, Stopwatch> theoryExplicitMaxConfiguredDelayStopwatches =
            new ConcurrentDictionary<int, Stopwatch>();
        private static readonly ConcurrentDictionary<int, Stopwatch> theoryExplicitDelayPropertyStopwatches =
            new ConcurrentDictionary<int, Stopwatch>();
        private static readonly ConcurrentDictionary<int, Stopwatch> theoryExplicitDelayStopwatches =
            new ConcurrentDictionary<int, Stopwatch>();

        private static int factRetryCount;
        private static int explicitRetryCount;
        private static Stopwatch configuredDelayStopwatch;
        private static Stopwatch explicitMaxConfiguredDelayStopwatch;
        private static Stopwatch explicitDelayPropertyStopwatch;
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

        [RetryTheory(5)]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_ExplicitMaxRetries_OverrideConfiguredValue(int id)
        {
            int actual = explicitTheoryRetryCounts.AddOrUpdate(id, 1, (_, retries) => retries + 1);

            actual.Should().Be(5);
        }

        [RetryFact]
        public void RetryFact_UsesConfiguredDelayBetweenRetries()
        {
            configuredDelayStopwatch = configuredDelayStopwatch ?? Stopwatch.StartNew();

            configuredDelayStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(120);
        }

        [RetryTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_UsesConfiguredDelayBetweenRetries(int id)
        {
            Stopwatch sw = theoryConfiguredDelayStopwatches.GetOrAdd(id, _ => Stopwatch.StartNew());

            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(120);
        }

        [RetryFact(5)]
        public void RetryFact_ExplicitMaxRetries_UsesConfiguredDelayBetweenRetries()
        {
            explicitMaxConfiguredDelayStopwatch = explicitMaxConfiguredDelayStopwatch ?? Stopwatch.StartNew();

            explicitMaxConfiguredDelayStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(170);
        }

        [RetryTheory(5)]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_ExplicitMaxRetries_UsesConfiguredDelayBetweenRetries(int id)
        {
            Stopwatch sw = theoryExplicitMaxConfiguredDelayStopwatches.GetOrAdd(id, _ => Stopwatch.StartNew());

            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(170);
        }

        [RetryFact(DelayBetweenRetriesMs = 100)]
        public void RetryFact_DelayBetweenRetriesProperty_OverridesConfiguredValue()
        {
            explicitDelayPropertyStopwatch = explicitDelayPropertyStopwatch ?? Stopwatch.StartNew();

            explicitDelayPropertyStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(250);
        }

        [RetryTheory(DelayBetweenRetriesMs = 100)]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_DelayBetweenRetriesProperty_OverridesConfiguredValue(int id)
        {
            Stopwatch sw = theoryExplicitDelayPropertyStopwatches.GetOrAdd(id, _ => Stopwatch.StartNew());

            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(250);
        }

        [RetryFact(5, 100)]
        public void RetryFact_ExplicitDelayBetweenRetries_OverridesConfiguredValue()
        {
            explicitDelayStopwatch = explicitDelayStopwatch ?? Stopwatch.StartNew();

            explicitDelayStopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(350);
        }

        [RetryTheory(5, 100)]
        [InlineData(0)]
        [InlineData(1)]
        public void RetryTheory_ExplicitDelayBetweenRetries_OverridesConfiguredValue(int id)
        {
            Stopwatch sw = theoryExplicitDelayStopwatches.GetOrAdd(id, _ => Stopwatch.StartNew());

            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(350);
        }
    }
}
