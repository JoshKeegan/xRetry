using xRetry.SpecFlow;
using xRetry.SpecFlow.Parsers;
using Xunit;

namespace UnitTests.SpecFlow.Parsers
{
    public class RetryTagParserTests
    {
        [Fact]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            // Arrange
            var expected = new RetryTag(3, 5000);

            // Act
            var actual = getParser().Parse(null);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_NoParams_CorrectResult()
        {
            // Arrange
            var parser = getParser();
            var expected = new RetryTag(3, 5000);

            // Act
            var actual = parser.Parse("retry");

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
            var parser = getParser();
            var expected = new RetryTag(maxRetries, 5000);

            // Act
            var actual = parser.Parse(tag);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("retry(5,100)", 5, 100)]
        [InlineData("Retry(5,100)", 5, 100)]
        [InlineData("RETRY(5,100)", 5, 100)]
        [InlineData("rEtRy(5,100)", 5, 100)]
        [InlineData("retry(765,87)", 765, 87)]
        public void Parse_MaxRetriesAndDelayBetweenRetriesMs_ReturnsCorrectResult(
            string tag,
            int maxRetries,
            int delayBetweenRetriesMs)
        {
            // Arrange
            var parser = getParser();
            var expected = new RetryTag(maxRetries, delayBetweenRetriesMs);

            // Act
            var actual = parser.Parse(tag);

            // Assert
            Assert.Equal(expected, actual);
        }

        private RetryTagParser getParser()
        {
            return new(new RetrySettings());
        }
    }
}