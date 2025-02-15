using FluentAssertions;
using xRetry.v3;

namespace UnitTests.v3.Facts
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
