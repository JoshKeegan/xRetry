using xRetry;
using Xunit;

namespace UnitTests.Facts
{
    public class RetryFactMaxRetriesTests
    {
        private static int fiveRunsNumCalls = 0;

        [RetryFact(5)]
        public void FiveRuns_Reaches5()
        {
            fiveRunsNumCalls++;

            Assert.Equal(5, fiveRunsNumCalls);
        }
    }
}
