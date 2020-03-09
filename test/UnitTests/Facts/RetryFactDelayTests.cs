using System.Diagnostics;
using xRetry;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Facts
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
            Assert.True(elapsedMs >= 90);
        }
    }
}
