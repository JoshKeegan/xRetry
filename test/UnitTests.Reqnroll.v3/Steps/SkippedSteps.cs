using Reqnroll;
using Xunit;

namespace UnitTests.Reqnroll.v3.Steps
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
