using FluentAssertions;
using UnitTests.v3.TestClasses;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3.Facts
{
    public class RetryFactRuntimeSkipTests
    {
        [RetryFact(SkipExceptions = new[] { typeof(TestException) })]
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

            skippedNumCalls.Should().Be(1);

            Assert.Skip("some reason");
        }
    }
}
