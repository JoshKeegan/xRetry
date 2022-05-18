using FluentAssertions;
using xRetry;

namespace UnitTests.Facts
{
    public class RetryFactDefaultTests
    {
        private static int defaultNumCalls = 0;

        [RetryFact]
        public void Default_Reaches3()
        {
            defaultNumCalls++;
            
            defaultNumCalls.Should().Be(3);
        }
    }
}
