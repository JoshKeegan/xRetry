using System.Collections.Generic;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests.Theories
{
    public class RetryTheoryDefaultTests
    {
        // testId => numCalls
        private static readonly Dictionary<int, int> defaultNumCalls = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 0 }
        };

        [RetryTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void Default_Reaches3(int id)
        {
            defaultNumCalls[id]++;

            defaultNumCalls[id].Should().Be(3);
        }
    }
}
