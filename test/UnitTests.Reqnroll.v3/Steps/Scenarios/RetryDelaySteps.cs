using System.Collections.Concurrent;
using System.Diagnostics;
using Reqnroll;
using UnitTests.Reqnroll.v3.TestClasses;
using Xunit;

namespace UnitTests.Reqnroll.v3.Steps.Scenarios
{
    [Binding]
    public class RetryDelaySteps
    {
        // scenarioId => numCalls
        private static readonly ConcurrentDictionary<ScenarioId, Stopwatch> sws =
            new ConcurrentDictionary<ScenarioId, Stopwatch>();

        private readonly ScenarioId scenarioId;

        public RetryDelaySteps(ScenarioId scenarioId)
        {
            this.scenarioId = scenarioId;
        }

        [When(@"I start the stopwatch if not already started")]
        public void WhenIStartTheStopwatchIfNotAlreadyStarted() =>
            sws.AddOrUpdate(scenarioId, _ =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                return sw;
            }, (_, sw) =>
            {
                sw.Start();
                return sw;
            });

        [Then(@"the elapsed time on the stopwatch is greater than or equal to (\d+)ms")]
        public void TheElapsedTimeOnTheStopwatchIsGreaterThanOrEqualToMs(int minElapsedMs)
        {
            Assert.True(sws.TryGetValue(scenarioId, out Stopwatch sw),
                $"Scenario never ran in the current scenario ({scenarioId})");
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }
    }
}
