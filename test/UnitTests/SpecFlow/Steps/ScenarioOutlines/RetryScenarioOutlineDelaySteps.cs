using System.Collections.Concurrent;
using System.Diagnostics;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.ScenarioOutlines
{
    [Binding]
    public class RetryScenarioOutlineDelaySteps
    {
        private static readonly ConcurrentDictionary<int, Stopwatch> stopwatches =
            new ConcurrentDictionary<int, Stopwatch>();

        [When(@"I start the stopwatch for test (\d+) if not already started")]
        public void WhenIStartTheStopwatchForTestIfNotAlreadyStarted(int n)
        {
            stopwatches.GetOrAdd(n, _ =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                return sw;
            });
        }

        [Then(@"the elapsed time on the stopwatch for test (\d+) is greater than or equal to (\d+)ms")]
        public void ThenTheElapsedTimeOnTheStopwatchForTestIsGreaterThanOrEqualToMs(int n, int minElapsedMs)
        {
            Assert.True(stopwatches.TryGetValue(n, out Stopwatch sw), "Scenario example never ran");
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }
    }
}
