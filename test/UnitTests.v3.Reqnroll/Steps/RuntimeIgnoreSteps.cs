using Reqnroll;
using Reqnroll.UnitTestProvider;

namespace UnitTests.Reqnroll.v3.Steps
{
    [Binding]
    public class RuntimeIgnoreSteps
    {
        private readonly IUnitTestRuntimeProvider unitTestRuntimeProvider;

        public RuntimeIgnoreSteps(IUnitTestRuntimeProvider unitTestRuntimeProvider)
        {
            this.unitTestRuntimeProvider = unitTestRuntimeProvider;
        }

        [When(@"I ignore this test")]
        public void WhenIIgnoreThisTest()
        {
            unitTestRuntimeProvider.TestIgnore("Ignored at runtime");
        }
    }
}