using System;
using AutoFixture;
using FluentAssertions;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3
{
    public class RetryTheoryAttributeTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_LessThanOneMaxRetries_ShouldThrow(int maxRetries)
        {
            Action act = () => new RetryTheoryAttribute(maxRetries);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_NegativeDelayBetweenRetries_ShouldThrow(int delayBetweenRetriesMs)
        {
            Action act = () => new RetryTheoryAttribute(delayBetweenRetriesMs: delayBetweenRetriesMs);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
