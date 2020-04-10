using System.Threading.Tasks;
using xRetry;

namespace UnitTests.Facts
{
    public class AsyncNonDeadlocking
    {
        // Usage: set maxParallelThreads to 1 in xunit.runner.json and run this test to check if it passes
        // TODO: Can xunit be told to run this specific test with that setting?
        //  If not, perhaps make this its own test assembly so we can have a separate suite of tests with maxParallelThreads 1
        //  Or just script running tests with and without that set during build, so we run all tests in both configurations

        [RetryFact(1)]
        public async Task AwaitWithinAsyncTask_RunsToCompletion()
        {
            // If .Result is being used further up the call stack and there is only a single thread available, this will cause a deadlock
            await Task.Delay(10);
        }

        [RetryFact(1)]
        public async void AwaitWithinAsyncVoid_RunsToCompletion()
        {
            // If .Result is being used further up the call stack and there is only a single thread available, this will cause a deadlock
            await Task.Delay(10);
        }
    }
}
