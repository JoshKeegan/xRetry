using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    internal static class RetryTestCaseRunner
    {
        public static async Task<RunSummary> RunAsync(
            IRetryableTestCase testCase,
            IMessageSink diagnosticMessageSink, 
            IMessageBus messageBus,
            object[] constructorArguments, 
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            for (int i = 1; ; i++)
            {
                // Prevent messages from the test run from being passed through, as we don't want 
                //  a message to mark the test as failed when we're going to retry it
                using (BlockingMessageBus blockingMessageBus = new BlockingMessageBus(messageBus))
                {
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage("Running test \"{0}\" attempt ({1}/{2})",
                        testCase.DisplayName, i, testCase.MaxRetries));

                    RunSummary summary = await RunSingleAsync(testCase, blockingMessageBus, constructorArguments,
                        aggregator, cancellationTokenSource).ConfigureAwait(false);

                    // If we succeeded, or we've reached the max retries return the result
                    if (summary.Failed == 0 || i == testCase.MaxRetries)
                    {
                        blockingMessageBus.Flush();
                        return summary;
                    }
                    // Otherwise log that we've had a failed run and will retry
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        "Test \"{0}\" failed but is set to retry ({1}/{2}) . . .", testCase.DisplayName, i,
                        testCase.MaxRetries));

                    // If there is a delay between test attempts, apply it now
                    if (testCase.DelayBetweenRetriesMs > 0)
                    {
                        diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                            "Test \"{0}\" attempt ({1}/{2}) delayed by {3}ms. Waiting . . .", testCase.DisplayName, i,
                            testCase.MaxRetries, testCase.DelayBetweenRetriesMs));
                        await Task.Delay(testCase.DelayBetweenRetriesMs, cancellationTokenSource.Token)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        private static Task<RunSummary> RunSingleAsync(
            IXunitTestCase testCase,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) =>
            new XunitTestCaseRunner(testCase, testCase.DisplayName, testCase.SkipReason, constructorArguments,
                testCase.TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
    }
}
