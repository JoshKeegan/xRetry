using System;
using AutoFixture;
using FluentAssertions;
using xRetry;
using Xunit;

namespace UnitTests
{
    public class RetryTheoryAttributeTests
    {
        [Fact]
        public void Ctor_Empty_NoSkipOnExceptions()
        {
            // Arrange & Act
            RetryTheoryAttribute attr = new RetryTheoryAttribute();

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
            RetryTheoryAttribute attr = new RetryTheoryAttribute(expected);

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
            RetryTheoryAttribute attr = new RetryTheoryAttribute(fixture.Create<int>(), fixture.Create<int>(), expected);

            // Assert
            attr.SkipOnExceptions.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_NonExceptionTypes_ShouldThrow() =>
            Assert.Throws<ArgumentException>(() => new RetryTheoryAttribute(typeof(RetryFactAttributeTests)));

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_LessThanOneMaxRetries_ShouldThrow(int maxRetries) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryTheoryAttribute(maxRetries));

        [Theory]
        [InlineData(-1)]
        [InlineData(-1337)]
        public void Ctor_NegativeDelayBetweenRetries_ShouldThrow(int delayBetweenRetriesMs) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryTheoryAttribute(delayBetweenRetriesMs: delayBetweenRetriesMs));
    }
}
