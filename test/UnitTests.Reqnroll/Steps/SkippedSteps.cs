using Reqnroll;
using Xunit;

namespace UnitTests.Reqnroll.Steps
{
    [Binding]
    public class SkippedSteps
    {
        [Then(@"fail because (.+)")]
        public void ThenFail(string reason)
        {
            Assert.Fail(reason);
        }
    }
}
