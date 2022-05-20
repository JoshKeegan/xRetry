using System.Collections.Generic;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests.Theories
{
    public class RetryTheoryMaxRetriesTests
    {
        // testId => numCalls
        private static readonly Dictionary<int, int> fiveRunsNumCalls = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 0 }
        };

        [RetryTheory(5)]
        [InlineData(0)]
        [InlineData(1)]
        public void FiveRuns_Reaches5(int id)
        {
            fiveRunsNumCalls[id]++;

            fiveRunsNumCalls[id].Should().Be(5);
        }
    }
}
