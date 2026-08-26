using FluentAssertions;
using Xunit;

namespace UnitTests.SpecFlow
{
    public class RetryUntaggedAttributeTests
    {
        [Fact]
        public void FactDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
        {
            var attribute = new xRetry.SpecFlow.RetryUntaggedFactAttribute();

            attribute.MaxRetries.Should().Be(1);
        }

        [Fact]
        public void TheoryDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
        {
            var attribute = new xRetry.SpecFlow.RetryUntaggedTheoryAttribute();

            attribute.MaxRetries.Should().Be(1);
        }
    }
}
