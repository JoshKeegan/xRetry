using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.Scenarios
{
    [Binding]
    public class RetrySteps
    {
        private static int retryCount = 0;

        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount()
        {
            retryCount++;
        }

        [Then(@"the result should be (\d+)")]
        public void ThenTheResultShouldBe(int expected)
        {
            Assert.Equal(expected, retryCount);
        }
    }
}
