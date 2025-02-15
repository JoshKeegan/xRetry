using System.Collections.Generic;
using FluentAssertions;
using UnitTests.v3.TestClasses;
using xRetry.v3;
using Xunit;
using Skip = xRetry.v3.Skip;

namespace UnitTests.v3.Theories
{
    public class RetryTheoryRuntimeSkipNonSerializableDataTests
    {
        [RetryTheory]
        [MemberData(nameof(GetTestData))]
        public void SkipAtRuntime(NonSerializableTestData _)
        {
            // Note: All we're doing with this test is checking that the rest of the test doesn't get run
            //  checking it's skipped (and doesn't pass) would need to be done manually.
            Skip.Always();

            Assert.Fail("Should have been skipped . . .");
        }

        [RetryTheory(typeof(TestException))]
        [MemberData(nameof(GetTestData))]
        public void CustomException_SkipsAtRuntime(NonSerializableTestData _)
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
        [MemberData(nameof(GetTestData))]
        public void Skip_DoesNotRetry(NonSerializableTestData nonSerializableWrapper)
        {
            // Assertion would fail on subsequent attempts, before reaching the skip
            skippedNumCalls[nonSerializableWrapper.Id]++;

            skippedNumCalls[nonSerializableWrapper.Id].Should().Be(1);

            Skip.Always();
        }

        public static IEnumerable<object[]> GetTestData() => new[]
        {
            new object[] { new NonSerializableTestData(0) },
            new object[] { new NonSerializableTestData(1) }
        };
    }
}
