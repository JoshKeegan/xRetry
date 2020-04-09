using System.Diagnostics;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.Scenarios
{
    [Binding]
    public class RetryDelaySteps
    {
        private static Stopwatch sw = null;

        [When(@"I start the stopwatch if not already started")]
        public void WhenIStartTheStopwatchIfNotAlreadyStarted()
        {
            if (sw == null)
            {
                sw = new Stopwatch();
                sw.Start();
            }
        }

        [Then(@"the elapsed time on the stopwatch is greater than or equal to (\d+)ms")]
        public void TheElapsedTimeOnTheStopwatchIsGreaterThanOrEqualToMs(int minElapsedMs)
        {
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }
    }
}
