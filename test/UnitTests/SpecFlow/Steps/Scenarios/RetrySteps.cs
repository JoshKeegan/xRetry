using System.Collections.Concurrent;
using TechTalk.SpecFlow;
using UnitTests.SpecFlow.TestClasses;
using Xunit;

namespace UnitTests.SpecFlow.Steps.Scenarios
{
    [Binding]
    public class RetrySteps
    {
        // scenarioId => numCalls
        private static readonly ConcurrentDictionary<ScenarioId, int> retryCount = new ConcurrentDictionary<ScenarioId, int>();

        private readonly ScenarioId scenarioId;

        public RetrySteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount() => retryCount.AddOrUpdate(scenarioId, 1, (_, v) => v + 1);

        [Then(@"the result should be (\d+)")]
        public void ThenTheResultShouldBe(int expected)
        {
            Assert.True(retryCount.TryGetValue(scenarioId, out int actual),
                $"Scenario never ran in the current scenario ({scenarioId})");
            Assert.Equal(expected, actual);
        }
    }
}
