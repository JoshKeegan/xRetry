using System;
using System.Collections.Generic;
using System.Text;
using xRetry.SpecFlow.Parsers;
using Xunit;

namespace UnitTests.SpecFlow.Parsers
{
    public class RetryTagParserTests
    {
        [Fact]
        public void Parse_Null_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => getParser().Parse(null));

        [Fact]
        public void Parse_NoParams_CorrectResult()
        {
            // Arrange
            RetryTagParser parser = getParser();
            RetryTag expected = new RetryTag(null, null);

            // Act
            RetryTag actual = parser.Parse("retry");

            // Assert
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("retry(5,100)", 5, 100)]
        [InlineData("Retry(5,100)", 5, 100)]
        [InlineData("RETRY(5,100)", 5, 100)]
        [InlineData("rEtRy(5,100)", 5, 100)]
        [InlineData("retry(5, 100)", 5, 100)]
        [InlineData("retry(5,  100)", 5, 100)]
        [InlineData("retry(765, 87)", 765, 87)]
        public void Parse_MaxRetriesAndDelayBetweenRetriesMs_ReturnsCorrectResult(string tag, int maxRetries,
            int delayBetweenRetriesMs)
        {
            // Arrange
            RetryTagParser parser = getParser();
            RetryTag expected = new RetryTag(maxRetries, delayBetweenRetriesMs);

            // Act
            RetryTag actual = parser.Parse(tag);

            // Assert
            Assert.Equal(expected, actual);
        }

        private RetryTagParser getParser() => new RetryTagParser();
    }
}
