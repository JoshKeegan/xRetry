using Xunit;
using xRetry.Reqnroll;

namespace InvalidConfigTests.v2
{
    public class InvalidConfigTests
    {
        [RetryUntaggedFact]
        public void ScenarioOne() { }

        [RetryUntaggedFact]
        public void ScenarioTwo() { }

        [RetryUntaggedTheory]
        [InlineData(1)]
        public void ScenarioOutline(int _) { }
    }
}
