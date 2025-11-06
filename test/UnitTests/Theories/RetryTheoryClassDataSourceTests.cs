using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests.Theories
{
    public class RetryTheoryClassDataSourceTests
    {
        private class ClassDataSource : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return [0];
                yield return [1];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        // testId => numCalls
        private static readonly Dictionary<int, int> defaultNumCalls = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 0 }
        };

        [RetryTheory]
        [ClassData(typeof(ClassDataSource))]
        public void Default_Reaches3(int id)
        {
            defaultNumCalls[id]++;

            defaultNumCalls[id].Should().Be(3);
        }
    }
}
