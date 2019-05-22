using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps
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

        [Then(@"the stopwatch elapsed milliseconds is greater than or equal to '(.*)'")]
        public void ThenTheStopwatchElapsedMillisecondsIsGreaterThan(int minElapsedMs)
        {
            Assert.True(sw.ElapsedMilliseconds >= minElapsedMs);
        }

    }
}
