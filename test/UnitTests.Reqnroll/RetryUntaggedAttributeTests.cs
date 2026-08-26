using FluentAssertions;
using Xunit;

namespace UnitTests.Reqnroll
{
    public class RetryUntaggedAttributeTests
    {
        [Fact]
        public void FactDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
        {
            var attribute = new xRetry.Reqnroll.RetryUntaggedFactAttribute();

            attribute.MaxRetries.Should().Be(1);
        }

        [Fact]
        public void TheoryDoesNotRetryUntaggedScenariosWhenConfigurationIsMissing()
        {
            var attribute = new xRetry.Reqnroll.RetryUntaggedTheoryAttribute();

            attribute.MaxRetries.Should().Be(1);
        }
    }
}
