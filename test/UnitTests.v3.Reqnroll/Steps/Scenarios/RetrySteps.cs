using System.Collections.Concurrent;
using Reqnroll;
using UnitTests.v3.Reqnroll.TestClasses;
using Xunit;

namespace UnitTests.v3.Reqnroll.Steps.Scenarios
{
    [Binding]
    public class RetrySteps
    {
        // scenarioId => numCalls
        private static readonly ConcurrentDictionary<ScenarioId, int> retryCount = new();

        private readonly ScenarioId scenarioId;

        public RetrySteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount()
        {
            retryCount.AddOrUpdate(scenarioId, 1, (_, v) => v + 1);
        }

        [Then(@"the result should be (\d+)")]
        public void ThenTheResultShouldBe(int expected)
        {
            Assert.True(retryCount.TryGetValue(scenarioId, out var actual),
                $"Scenario never ran in the current scenario ({scenarioId})");
            Assert.Equal(expected, actual);
        }
    }
}