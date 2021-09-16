using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AutoFixture;
using FluentAssertions;
using xRetry.Exceptions;
using Xunit;

namespace UnitTests.Exceptions
{
    public class SkipTestExceptionTests
    {
        [Fact]
        public void Serialisation_RoundTrip_RetainsData()
        {
            // Arrange
            Fixture fixture = new Fixture();
            SkipTestException expected = new SkipTestException(fixture.Create<string>());

            // Act
            SkipTestException actual;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream s = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable 618
                formatter.Serialize(s, expected);
                s.Position = 0;
                actual = (SkipTestException) formatter.Deserialize(s);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
#pragma warning restore 618
            }

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
