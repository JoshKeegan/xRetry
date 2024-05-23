using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using TechTalk.SpecFlow;
using UnitTests.SpecFlow.TestClasses;
using Xunit;

namespace UnitTests.SpecFlow.Steps.ScenarioOutlines
{
    [Binding]
    public class RetryScenarioOutlineDelaySteps
    {
        // (scenarioId, testId) => numCalls
        private static readonly ConcurrentDictionary<Tuple<ScenarioId, int>, Stopwatch> stopwatches =
            new ConcurrentDictionary<Tuple<ScenarioId, int>, Stopwatch>();

        private readonly ScenarioId scenarioId;

        public RetryScenarioOutlineDelaySteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I start the stopwatch for test (\d+) if not already started")]
        public void WhenIStartTheStopwatchForTestIfNotAlreadyStarted(int n) =>
            stopwatches.GetOrAdd(
                Tuple.Create(scenarioId, n), _ =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    return sw;
                });

        [Then(@"the elapsed time on the stopwatch for test (\d+) is greater than or equal to (\d+)ms")]
        public void ThenTheElapsedTimeOnTheStopwatchForTestIsGreaterThanOrEqualToMs(int n, int minElapsedMs)
        {
            Assert.True(stopwatches.TryGetValue(Tuple.Create(scenarioId, n), out Stopwatch sw),
                $"Scenario never ran in the current scenario outline ({scenarioId}, Test ID {n})");
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }
    }
}
