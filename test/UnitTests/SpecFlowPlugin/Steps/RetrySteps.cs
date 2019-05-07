using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlowPlugin.Steps
{
    [Binding]
    public class RetrySteps
    {
        private static int _retryCount = 0;

        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount()
        {
            _retryCount++;
        }

        [Then(@"the result should be (.*)")]
        public void ThenTheResultShouldBe(int expected)
        {
            Assert.Equal(expected, _retryCount);
        }
    }
}
