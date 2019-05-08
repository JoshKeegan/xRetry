using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlowPlugin.Steps
{
    [Binding]
    public class RetryDefaultSteps
    {
        private static int retryCount = 0;

        [When(@"I increment the default retry count")]
        public void WhenIIncrementTheDefaultRetryCount()
        {
            retryCount++;
        }

        [Then(@"the default result should be (.*)")]
        public void ThenTheDefaultResultShouldBe(int expected)
        {
            Assert.Equal(expected, retryCount);
        }
    }
}
