using System.Collections.Concurrent;
using System.Diagnostics;
using Reqnroll;
using UnitTests.v3.Reqnroll.TestClasses;
using Xunit;

namespace UnitTests.v3.Reqnroll.Steps.Scenarios
{
    [Binding]
    public class RetryDelaySteps
    {
        // scenarioId => numCalls
        private static readonly ConcurrentDictionary<ScenarioId, Stopwatch> sws = new();

        private readonly ScenarioId scenarioId;

        public RetryDelaySteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I start the stopwatch if not already started")]
        public void WhenIStartTheStopwatchIfNotAlreadyStarted()
        {
            sws.AddOrUpdate(scenarioId, _ =>
            {
                var sw = new Stopwatch();
                sw.Start();
                return sw;
            }, (_, sw) =>
            {
                sw.Start();
                return sw;
            });
        }

        [Then(@"the elapsed time on the stopwatch is greater than or equal to (\d+)ms")]
        public void TheElapsedTimeOnTheStopwatchIsGreaterThanOrEqualToMs(int minElapsedMs)
        {
            Assert.True(sws.TryGetValue(scenarioId, out var sw),
                $"Scenario never ran in the current scenario ({scenarioId})");
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }
    }
}