using FluentAssertions;
using System;
using xRetry.Reqnroll.Parsers;
using Xunit;

namespace UnitTests.Reqnroll.Parsers
{
    public class RetryTagParserTests
    {
        [Fact]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Action act = () => getParser().Parse(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Parse_NoParams_CorrectResult()
        {
            // Arrange
            RetryTagParser parser = getParser();
            RetryTag expected = new RetryTag(null, null);

            // Act
            RetryTag actual = parser.Parse("retry");

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
            RetryTagParser parser = getParser();
            RetryTag expected = new RetryTag(maxRetries, null);

            // Act
            RetryTag actual = parser.Parse(tag);

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
            RetryTagParser parser = getParser();
            RetryTag expected = new RetryTag(maxRetries, delayBetweenRetriesMs);

            // Act
            RetryTag actual = parser.Parse(tag);

            // Assert
            actual.Should().Be(expected);
        }

        private RetryTagParser getParser() => new RetryTagParser();
    }
}
