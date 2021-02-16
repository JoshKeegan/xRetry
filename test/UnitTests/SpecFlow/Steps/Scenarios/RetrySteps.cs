using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps.Scenarios
{
    [Binding]
    public class RetrySteps
    {
        private static int retryCount = 0;
        private static int retryGlobalCount = 0;

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

        [When(@"I increment the global retry count")]
        public void WhenIIncrementTheGlobalRetryCount()
        {
            retryGlobalCount++;
        }

        [Then(@"the global result should be (\d+)")]
        public void ThenTheGlobalResultShouldBe(int expected)
        {
            Assert.Equal(expected, retryGlobalCount);
        }
    }
}
