using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XunitRetry;

namespace UnitTests
{
    public class RetryFactTests
    {
        private static int _fiveRunsNumCalls = 0;

        [RetryFact(5)]
        public void FiveRuns_Reaches5()
        {
            _fiveRunsNumCalls++;

            Assert.Equal(5, _fiveRunsNumCalls);
        }
    }
}
