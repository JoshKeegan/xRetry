using System.Threading.Tasks;
using xRetry.v3;

namespace UnitTests.SingleThreaded.v3.Facts
{
    public class AsyncNonDeadlocking
    {
        [RetryFact(1)]
        public async Task AwaitWithinAsyncTask_RunsToCompletion()
        {
            // If .Result is being used further up the call stack and there is only a single thread available, this will cause a deadlock
            await Task.Delay(10);
        }

        [RetryFact(1)]
        public async ValueTask AwaitWithinAsyncValueTask_RunsToCompletion()
        {
            // If .Result is being used further up the call stack and there is only a single thread available, this will cause a deadlock
            await Task.Delay(10);
        }
    }
}
