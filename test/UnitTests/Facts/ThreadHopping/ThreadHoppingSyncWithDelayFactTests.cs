using FluentAssertions;
using xRetry;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Facts.ThreadHopping
{
    public class ThreadHoppingSyncWithDelayFactTests : IClassFixture<ThreadHoppingFixture>
    {
        private const int NUM_ITERATIONS = 10;

        private readonly ThreadHoppingFixture fixture;
        private readonly ITestOutputHelper testOutputHelper;

        public ThreadHoppingSyncWithDelayFactTests(ThreadHoppingFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
        }

        [RetryFact(NUM_ITERATIONS, 10)]
        public void SyncFact_WithDelay_AlwaysRunsOnSameThread()
        {
            fixture.AddAttempt();

            fixture.NumAttempts.Should().Be(NUM_ITERATIONS);

            fixture.Assert(testOutputHelper);
        }
    }
}
