using FluentAssertions;
using Xunit;

namespace UnitTests.v3.Reqnroll;

public class RetryUntaggedAttributeTests
{
    [Fact]
    public void FactDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
    {
        var attribute = new xRetry.v3.Reqnroll.RetryUntaggedFactAttribute();

        attribute.MaxRetries.Should().Be(1);
    }

    [Fact]
    public void TheoryDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
    {
        var attribute = new xRetry.v3.Reqnroll.RetryUntaggedTheoryAttribute();

        attribute.MaxRetries.Should().Be(1);
    }
}
