using Xunit;
using XunitRetry;

namespace UnitTests
{
    public class RetryFactTests
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
