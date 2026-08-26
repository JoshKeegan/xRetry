using Xunit;
using xRetry.v3.Reqnroll;

namespace InvalidConfigTests.v3;

public class InvalidConfigTests
{
    [RetryUntaggedFact(DisplayName = "Scenario one")]
    public void ScenarioOne() { }

    [RetryUntaggedFact(DisplayName = "Scenario two")]
    public void ScenarioTwo() { }

    [RetryUntaggedTheory(DisplayName = "Scenario outline")]
    [InlineData(1)]
    public void ScenarioOutline(int _) { }
}
