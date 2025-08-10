using System.Threading.Tasks;
using FluentAssertions;
using xRetry.v3;
using Xunit;

namespace UnitTests.v3.Facts.ThreadHopping
{
    public class ThreadHoppingAsyncTaskNoDelayFactTests : IClassFixture<ThreadHoppingFixture>
    {
        private const int NUM_ITERATIONS = 10;

        private readonly ThreadHoppingFixture fixture;
        private readonly ITestOutputHelper testOutputHelper;

        public ThreadHoppingAsyncTaskNoDelayFactTests(ThreadHoppingFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
        }

        [RetryFact(NUM_ITERATIONS)]
#pragma warning disable 1998
        public async Task SyncFact_NoDelay_AlwaysRunsOnSameThread()
#pragma warning restore 1998
        {
            // No await within the test method. Having that would potentially cause the test to thread hop, and if the user is doing that
            //  then they must be ok with thread hopping anyway

            fixture.AddAttempt();

            fixture.NumAttempts.Should().Be(NUM_ITERATIONS);

            fixture.Assert(testOutputHelper);
        }
    }
}
