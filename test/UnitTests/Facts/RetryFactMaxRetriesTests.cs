using FluentAssertions;
using xRetry;

namespace UnitTests.Facts
{
    public class RetryFactMaxRetriesTests
    {
        private static int fiveRunsNumCalls = 0;

        [RetryFact(5)]
        public void FiveRuns_Reaches5()
        {
            fiveRunsNumCalls++;
            
            fiveRunsNumCalls.Should().Be(5);
        }
    }
}
