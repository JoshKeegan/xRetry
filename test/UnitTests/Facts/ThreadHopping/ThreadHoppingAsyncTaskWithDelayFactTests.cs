using FluentAssertions;
using System.Threading.Tasks;
using xRetry;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Facts.ThreadHopping
{
    public class ThreadHoppingAsyncTaskWithDelayFactTests : IClassFixture<ThreadHoppingFixture>
    {
        private const int NUM_ITERATIONS = 10;

        private readonly ThreadHoppingFixture fixture;
        private readonly ITestOutputHelper testOutputHelper;

        public ThreadHoppingAsyncTaskWithDelayFactTests(ThreadHoppingFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
        }

        // Note that the following test case only exists for "async Task", not "async void" as xunit can thread hop for "async void" between the
        //  test fixture being instantiated and the test being run, so the single-thread behaviour we're trying to ensure here only exists for
        //  "async Task" tests.

        [RetryFact(NUM_ITERATIONS, 10)]
#pragma warning disable 1998
        public async Task SyncFact_WithDelay_AlwaysRunsOnSameThread()
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
