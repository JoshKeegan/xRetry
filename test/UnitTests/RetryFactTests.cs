using xRetry;
using Xunit;

namespace UnitTests
{
    public class RetryFactTests
    {
        private static int defaultNumCalls = 0;

        [RetryFact]
        public void Default_Reaches3()
        {
            defaultNumCalls++;

            Assert.Equal(3, defaultNumCalls);
        }

        private static int fiveRunsNumCalls = 0;

        [RetryFact(5)]
        public void FiveRuns_Reaches5()
        {
            fiveRunsNumCalls++;

            Assert.Equal(5, fiveRunsNumCalls);
        }
    }
}
