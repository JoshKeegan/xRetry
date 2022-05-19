using System.Collections.Generic;
using FluentAssertions;
using UnitTests.TestClasses;
using xRetry;
using Xunit;

namespace UnitTests.Theories
{
    // Non-serializable test data is a special case during xunit test discovery.
    //  These usually get discovered at runtime. See https://github.com/xunit/xunit/blob/3f3f6929c301db40e9d5d8ced2bb494084f57c82/src/xunit.execution/Sdk/Frameworks/TheoryDiscoverer.cs#L75
    //  Note that this isn't the only case handled by that method as there are other cases that cannot be pre-enumerated,
    // but this test should prove we have a working implementation of CreateTestCasesForTheory, so gives high confidence
    // without worrying about every possible edge case.
    public class RetryTheoryNonSerializableDataTests
    {
        // testId => numCalls
        private static readonly Dictionary<int, int> defaultNumCalls = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 0 }
        };

        [RetryTheory]
        [MemberData(nameof(GetTestData))]
        public void Default_Reaches3(NonSerializableTestData nonSerializableWrapper)
        {
            defaultNumCalls[nonSerializableWrapper.Id]++;

            defaultNumCalls[nonSerializableWrapper.Id].Should().Be(3);
        }

        public static IEnumerable<object[]> GetTestData() => new[]
        {
            new object[] { new NonSerializableTestData(0) },
            new object[] { new NonSerializableTestData(1) }
        };
    }
}
