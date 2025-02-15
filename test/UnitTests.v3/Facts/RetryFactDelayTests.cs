using System.Diagnostics;
using FluentAssertions;
using xRetry.v3;
using Xunit.Abstractions;

namespace UnitTests.v3.Facts
{
    public class RetryFactDelayTests
    {
        private static Stopwatch sw = null;

        private readonly ITestOutputHelper testOutputHelper;

        public RetryFactDelayTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [RetryFact(2, 100)]
        public void TwoRuns100MsDelay_AtLeast90MsApart()
        {
            if (sw == null)
            {
                sw = new Stopwatch();
                sw.Start();
            }

            long elapsedMs = sw.ElapsedMilliseconds;
            testOutputHelper.WriteLine("Elapsed {0}ms", elapsedMs);

            elapsedMs.Should().BeGreaterOrEqualTo(90);
        }
    }
}
