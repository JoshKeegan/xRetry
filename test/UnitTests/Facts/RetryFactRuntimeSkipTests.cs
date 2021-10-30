using UnitTests.TestClasses;
using xRetry;
using Xunit;
using Skip = xRetry.Skip;

namespace UnitTests.Facts
{
    public class RetryFactRuntimeSkipTests
    {
        [RetryFact]
        public void SkipAtRuntime()
        {
            // Note: All we're doing with this test is checking that the rest of the test doesn't get run
            //  checking it's skipped (and doesn't pass) would need to be done manually.
            Skip.Always();

            Assert.True(false, "Should have been skipped . . .");
        }

        [RetryFact(typeof(TestException))]
        public void CustomException_SkipsAtRuntime()
        {
            throw new TestException();
        }

        private static int skippedNumCalls = 0;

        [RetryFact]
        public void Skip_DoesNotRetry()
        {
            // Assertion would fail on subsequent attempts, before reaching the skip
            skippedNumCalls++;
            Assert.Equal(1, skippedNumCalls);

            Skip.Always();
        }
    }
}
