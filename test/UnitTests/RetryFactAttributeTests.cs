using System;
using AutoFixture;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests
{
    public class RetryFactAttributeTests
    {
        [Fact]
        public void Ctor_Empty_NoSkipOnExceptions()
        {
            // Arrange & Act
            RetryFactAttribute attr = new RetryFactAttribute();

            // Assert
            attr.SkipOnExceptions.Should().BeEmpty();
        }

        [Fact]
        public void SkipOnExceptionsCtor_Exceptions_ShouldSave()
        {
            // Arrange
            Type[] expected = new[]
            {
                typeof(ArgumentException),
                typeof(ArgumentNullException)
            };

            // Act
            RetryFactAttribute attr = new RetryFactAttribute(expected);

            // Assert
            attr.SkipOnExceptions.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void FullCtr_Exceptions_ShouldSave()
        {
            // Arrange
            Fixture fixture = new Fixture();
            Type[] expected = new[]
            {
                typeof(ArgumentException),
                typeof(ArgumentNullException)
            };

            // Act
            RetryFactAttribute attr = new RetryFactAttribute(fixture.Create<int>(), fixture.Create<int>(), expected);

            // Assert
            attr.SkipOnExceptions.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_NonExceptionTypes_ShouldThrow()
        {
            var act = () => new RetryFactAttribute(typeof(RetryFactAttributeTests));

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_LessThanOneMaxRetries_ShouldThrow(int maxRetries)
        {
            var act = () => new RetryFactAttribute(maxRetries);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_NegativeDelayBetweenRetries_ShouldThrow(int delayBetweenRetriesMs)
        {
            var act = () => new RetryFactAttribute(delayBetweenRetriesMs: delayBetweenRetriesMs);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
