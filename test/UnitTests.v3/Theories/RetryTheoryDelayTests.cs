using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;
using xRetry.v3;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.v3.Theories
{
    public class RetryTheoryDelayTests
    {
        private static readonly ConcurrentDictionary<int, Stopwatch> stopwatches =
            new ConcurrentDictionary<int, Stopwatch>();

        private readonly ITestOutputHelper testOutputHelper;

        public RetryTheoryDelayTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [RetryTheory(2, 100)]
        [InlineData(0)]
        [InlineData(1)]
        public void TwoRuns100MsDelay_AtLeast90MsApart(int id)
        {
            Stopwatch newStopwatch = new Stopwatch();
            newStopwatch.Start();
            Stopwatch sw = stopwatches.GetOrAdd(id, newStopwatch);

            long elapsedMs = sw.ElapsedMilliseconds;
            testOutputHelper.WriteLine("Elapsed {0}ms in test ID {1}", elapsedMs, id);

            elapsedMs.Should().BeGreaterOrEqualTo(90);
        }
    }
}
