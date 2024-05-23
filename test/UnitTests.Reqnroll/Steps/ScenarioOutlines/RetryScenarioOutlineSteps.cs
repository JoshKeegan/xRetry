using System;
using System.Collections.Concurrent;
using Reqnroll;
using UnitTests.Reqnroll.TestClasses;
using Xunit;

namespace UnitTests.Reqnroll.Steps.ScenarioOutlines
{
    [Binding]
    public class RetryScenarioOutlineSteps
    {
        // (scenarioId, testId) => numCalls
        private static readonly ConcurrentDictionary<Tuple<ScenarioId, int>, int> numCalls =
            new ConcurrentDictionary<Tuple<ScenarioId, int>, int>();// (scenarioId, testId) => numCalls

        private readonly ScenarioId scenarioId;

        public RetryScenarioOutlineSteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I increment the retry count for test (\d+)")]
        public void WhenIIncrementTheRetryCountForTest(int n) =>
            numCalls.AddOrUpdate(Tuple.Create(scenarioId, n), 1, (_, v) => v + 1);

        [Then(@"the retry count for test (\d+) should be (\d+)")]
        public void ThenTheRetryCountForTestShouldBe(int n, int expected)
        {
            Assert.True(numCalls.TryGetValue(Tuple.Create(scenarioId, n), out int actual),
                $"Scenario never ran in the current scenario outline ({scenarioId}, Test ID {n})");
            Assert.Equal(expected, actual);
        }
    }
}
