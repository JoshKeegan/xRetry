using System.Collections.Concurrent;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.ScenarioOutlines
{
    [Binding]
    public class RetryScenarioOutlineSteps
    {
        // testId => numCalls
        private static readonly ConcurrentDictionary<int, int> numCalls = new ConcurrentDictionary<int, int>();

        [When(@"I increment the retry count for test (\d+)")]
        public void WhenIIncrementTheRetryCountForTest(int n)
        {
            numCalls.AddOrUpdate(n, 1, (_, v) => v + 1);
        }

        [Then(@"the retry count for test (\d+) should be (\d+)")]
        public void ThenTheRetryCountForTestShouldBe(int n, int expected)
        {
            Assert.True(numCalls.TryGetValue(n, out int actual), "Scenario example never ran");
            Assert.Equal(expected, actual);
        }
    }
}
