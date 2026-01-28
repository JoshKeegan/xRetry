using System;
using FluentAssertions;
using xRetry.v3.Reqnroll.Parsers;
using Xunit;

namespace UnitTests.v3.Reqnroll.Parsers
{
    public class RetryTagParserTests
    {
        [Fact]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Action act = () => GetParser().Parse(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Parse_NoParams_CorrectResult()
        {
            // Arrange
            var parser = GetParser();
            var expected = new RetryTag(null, null);

            // Act
            var actual = parser.Parse("retry");

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("retry(5)", 5)]
        [InlineData("Retry(5)", 5)]
        [InlineData("RETRY(5)", 5)]
        [InlineData("ReTrY(5)", 5)]
        public void Parse_MaxRetries_ReturnsCorrectResult(string tag, int maxRetries)
        {
            // Arrange
            var parser = GetParser();
            var expected = new RetryTag(maxRetries, null);

            // Act
            var actual = parser.Parse(tag);

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("retry(5,100)", 5, 100)]
        [InlineData("Retry(5,100)", 5, 100)]
        [InlineData("RETRY(5,100)", 5, 100)]
        [InlineData("rEtRy(5,100)", 5, 100)]
        [InlineData("retry(765,87)", 765, 87)]
        public void Parse_MaxRetriesAndDelayBetweenRetriesMs_ReturnsCorrectResult(string tag, int maxRetries,
            int delayBetweenRetriesMs)
        {
            // Arrange
            var parser = GetParser();
            var expected = new RetryTag(maxRetries, delayBetweenRetriesMs);

            // Act
            var actual = parser.Parse(tag);

            // Assert
            actual.Should().Be(expected);
        }

        private static RetryTagParser GetParser()
        {
            return new RetryTagParser();
        }
    }
}