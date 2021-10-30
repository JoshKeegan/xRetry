using System.Collections.Generic;
using UnitTests.TestClasses;
using xRetry;
using Xunit;
using Skip = xRetry.Skip;

namespace UnitTests.Theories
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

            Assert.True(false, "Should have been skipped . . .");
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
            Assert.Equal(1, skippedNumCalls[id]);

            Skip.Always();
        }
    }
}
