using System.Collections.Concurrent;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.ScenarioOutlines
{
    [Binding]
    public class RetryScenarioOutlineDefaultSteps
    {
        // testId => numCalls
        private static readonly ConcurrentDictionary<int, int> defaultNumCalls = new ConcurrentDictionary<int, int>();

        [When(@"I increment the default retry count for test (\d+)")]
        public void WhenIIncrementTheDefaultRetryCountForTest(int n)
        {
            defaultNumCalls.AddOrUpdate(n, 1, (_, v) => v + 1);
        }

        [Then(@"the default retry could for test (\d+) should be (\d+)")]
        public void ThenTheDefaultRetryCouldForTestShouldBe(int n, int expected)
        {
            Assert.True(defaultNumCalls.TryGetValue(n, out int actual), "Scenario example never ran");
            Assert.Equal(expected, actual);
        }
    }
}
