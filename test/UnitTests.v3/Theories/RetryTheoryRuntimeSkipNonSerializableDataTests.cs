using System.Collections.Generic;
using FluentAssertions;
using UnitTests.v3.TestClasses;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3.Theories
{
    // TODO: v3 - missing feature
    // public class RetryTheoryRuntimeSkipNonSerializableDataTests
    // {
    //     [RetryTheory(SkipExceptions = new[] { typeof(TestException) })]
    //     [MemberData(nameof(GetTestData))]
    //     public void CustomException_SkipsAtRuntime(NonSerializableTestData _)
    //     {
    //         throw new TestException();
    //     }
    //
    //     // testId => numCalls
    //     private static readonly Dictionary<int, int> skippedNumCalls = new Dictionary<int, int>()
    //     {
    //         { 0, 0 },
    //         { 1, 0 }
    //     };
    //
    //     [RetryTheory]
    //     [MemberData(nameof(GetTestData))]
    //     public void Skip_DoesNotRetry(NonSerializableTestData nonSerializableWrapper)
    //     {
    //         // Assertion would fail on subsequent attempts, before reaching the skip
    //         skippedNumCalls[nonSerializableWrapper.Id]++;
    //
    //         skippedNumCalls[nonSerializableWrapper.Id].Should().Be(1);
    //
    //         Assert.Skip("some reason");
    //     }
    //
    //     public static IEnumerable<object[]> GetTestData() => new[]
    //     {
    //         new object[] { new NonSerializableTestData(0) },
    //         new object[] { new NonSerializableTestData(1) }
    //     };
    // }
}
