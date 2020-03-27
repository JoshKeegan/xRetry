using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.SpecFlow.Steps
{
    [Binding]
    public class SkippedSteps
    {
        [Then(@"fail because (.+)")]
        public void ThenFail(string reason)
        {
            Assert.True(false, reason);
        }
    }
}
