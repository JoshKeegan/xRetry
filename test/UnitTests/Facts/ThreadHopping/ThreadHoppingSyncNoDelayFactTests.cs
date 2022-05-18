using FluentAssertions;
using xRetry;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Facts.ThreadHopping
{
    public class ThreadHoppingSyncNoDelayFactTests : IClassFixture<ThreadHoppingFixture>
    {
        private const int NUM_ITERATIONS = 1000;

        private readonly ThreadHoppingFixture fixture;
        private readonly ITestOutputHelper testOutputHelper;

        public ThreadHoppingSyncNoDelayFactTests(ThreadHoppingFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
        }

        [RetryFact(NUM_ITERATIONS)]
        public void SyncFact_NoDelay_AlwaysRunsOnSameThread()
        {
            fixture.AddAttempt();

            fixture.NumAttempts.Should().Be(NUM_ITERATIONS);

            fixture.Assert(testOutputHelper);
        }
    }
}
