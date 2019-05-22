using System;
using System.Collections.Generic;
using System.Text;
using xRetry;
using Xunit;

namespace UnitTests
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
