using TechTalk.SpecFlow.UnitTestProvider;

namespace xRetry.SpecFlow
{
    public class TestRuntimeProvider : IUnitTestRuntimeProvider
    {
        public bool DelayedFixtureTearDown => false;

        public void TestIgnore(string message)
        {
            Skip.Always(message);
        }

        // TODO: Implement to match existing Specflow (& also test I guess...)
        public void TestInconclusive(string message)
        {
            throw new System.NotImplementedException();
        }

        public void TestPending(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
