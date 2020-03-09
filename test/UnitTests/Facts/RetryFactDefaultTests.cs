using xRetry;
using Xunit;

namespace UnitTests.Facts
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
    }
}
