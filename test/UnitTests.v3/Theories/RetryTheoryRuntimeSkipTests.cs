using System.Collections.Generic;
using FluentAssertions;
using UnitTests.v3.TestClasses;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3.Theories
{
    public class RetryTheoryRuntimeSkipTests
    {
        [RetryTheory(SkipExceptions = new[] {typeof(TestException)})]
        [InlineData(0)]
        [InlineData(1)]
        public void CustomException_SkipsAtRuntime(int _)
        {
            throw new TestException();
        }

        // testId => numCalls
        private static readonly Dictionary<int, int> skippedNumCalls = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 0 }
        };

        [RetryTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void Skip_DoesNotRetry(int id)
        {
            // Assertion would fail on subsequent attempts, before reaching the skip
            skippedNumCalls[id]++;

            skippedNumCalls[id].Should().Be(1);

            Assert.Skip("some reason");
        }
    }
}
