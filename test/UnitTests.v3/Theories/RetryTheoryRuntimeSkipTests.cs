using System.Collections.Generic;
using FluentAssertions;
using UnitTests.v3.TestClasses;
using xRetry.v3;
using Xunit;
using Skip = xRetry.v3.Skip;

namespace UnitTests.v3.Theories
{
    public class RetryTheoryRuntimeSkipTests
    {
        [RetryTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void SkipAtRuntime(int _)
        {
            // Note: All we're doing with this test is checking that the rest of the test doesn't get run
            //  checking it's skipped (and doesn't pass) would need to be done manually.
            Skip.Always();

            Assert.Fail("Should have been skipped . . .");
        }

        [RetryTheory(typeof(TestException))]
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

            Skip.Always();
        }
    }
}
