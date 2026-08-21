using Reqnroll;
using Xunit;

namespace PackageTests.Reqnroll
{
    [Binding]
    public sealed class PackageSmokeSteps
    {
        private static int attempts;

        [When("the package smoke scenario is attempted")]
        public void WhenThePackageSmokeScenarioIsAttempted()
        {
            attempts++;
        }

        [Then("it succeeds on the second attempt")]
        public void ThenItSucceedsOnTheSecondAttempt()
        {
            Assert.Equal(2, attempts);
        }
    }
}
