using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlowPlugin.Steps
{
    [Binding]
    public class RetryDefaultSteps
    {
        private static int _retryCount = 0;

        [When(@"I increment the default retry count")]
        public void WhenIIncrementTheDefaultRetryCount()
        {
            _retryCount++;
        }

        [Then(@"the default result should be (.*)")]
        public void ThenTheDefaultResultShouldBe(int expected)
        {
            Assert.Equal(expected, _retryCount);
        }
    }
}
