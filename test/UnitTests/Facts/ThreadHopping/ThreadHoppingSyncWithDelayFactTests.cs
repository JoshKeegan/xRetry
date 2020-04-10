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
        public void SyncFact_NoDelay_AlwaysRunsOnSameThread()
        {
            fixture.AddAttempt();

            Assert.Equal(NUM_ITERATIONS, fixture.NumAttempts);

            fixture.Assert(testOutputHelper);
        }
    }
}
