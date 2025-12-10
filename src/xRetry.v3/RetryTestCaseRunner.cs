using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.v3;

namespace xRetry.v3
{
    // Note: public so that other xunit extensions can call this, allowing xRetry to be 
    //  integrated into them. e.g. Xunit.DependencyInjection.xRetry
    public static class RetryTestCaseRunner
    {
        /// <summary>
        /// Runs a retryable test case, handling any wait and retry logic between test runs, reporting statuses out to xunit etc...
        /// </summary>
        /// <param name="testCase">The test case to be retried</param>
        /// <param name="messageBus">The message bus xunit is listening for statuses to report on</param>
        /// <param name="cancellationTokenSource">The cancellation token source from xunit</param>
        /// <param name="fnRunSingle">(async) Lambda to run this test case once (without retries) - takes the blocking message bus and returns the test run result</param>
        /// <returns>Resulting run summary</returns>
        public static async ValueTask<RunSummary> Run(
            IRetryableTestCase testCase,
            IMessageBus messageBus,
            CancellationTokenSource cancellationTokenSource,
            Func<IMessageBus, ValueTask<RunSummary>> fnRunSingle)
        {
            var now = DateTimeOffset.UtcNow;
            messageBus.QueueMessage(ToTestCaseStarting(testCase));

            for (int i = 1; ; i++)
            {
                // Prevent messages from the test run from being passed through, as we don't want 
                //  a message to mark the test as failed when we're going to retry it
                using BlockingMessageBus blockingMessageBus = new BlockingMessageBus(messageBus);
                messageBus.QueueMessage(new DiagnosticMessage("Running test \"{0}\" attempt ({1}/{2})",
                    testCase.TestCaseDisplayName, i, testCase.MaxRetries));

                RunSummary summary = await fnRunSingle(blockingMessageBus);

                // If we succeeded, skipped, or we've reached the max retries return the result
                if (summary.Failed == 0 || i == testCase.MaxRetries)
                {
                    // If we have failed (after all retries, log that)
                    if (summary.Failed != 0)
                    {
                        messageBus.QueueMessage(new DiagnosticMessage(
                            "Test \"{0}\" has failed and been retried the maximum number of times ({1})",
                            testCase.TestCaseDisplayName, testCase.MaxRetries));
                    }

                    blockingMessageBus.Flush();
                    messageBus.QueueMessage(ToTestCaseFinished(testCase, summary, now));
                    return summary;
                }
                // Otherwise log that we've had a failed run and will retry
                messageBus.QueueMessage(new DiagnosticMessage(
                    "Test \"{0}\" failed but is set to retry ({1}/{2}) . . .", testCase.TestCaseDisplayName, i,
                    testCase.MaxRetries));

                // If there is a delay between test attempts, apply it now
                if (testCase.DelayBetweenRetriesMs > 0)
                {
                    messageBus.QueueMessage(new DiagnosticMessage(
                        "Test \"{0}\" attempt ({1}/{2}) delayed by {3}ms. Waiting . . .", testCase.TestCaseDisplayName,
                        i, testCase.MaxRetries, testCase.DelayBetweenRetriesMs));

                    // Don't await to prevent thread hopping.
                    //  If all of a users test cases in a collection/class are synchronous and expecting to not thread-hop
                    //  (because they're making use of thread static/thread local/managed thread ID to share data between tests rather than
                    //  a more modern async-friendly mechanism) then if a thread-hop were to happen here we'd get flickering tests.
                    //  SpecFlow relies on this as they use the managed thread ID to separate instances of some of their internal classes, which caused
                    //  a this problem for xRetry.SpecFlow: https://github.com/JoshKeegan/xRetry/issues/18
                    Task.Delay(testCase.DelayBetweenRetriesMs, cancellationTokenSource.Token).Wait();
                }
            }
        }

        public static TestCaseStarting ToTestCaseStarting(IRetryableTestCase testCase)
        {
            var assemblyUniqueID = testCase.TestCollection.TestAssembly.UniqueID;
            var testCollectionUniqueID = testCase.TestCollection.UniqueID;
            var testCaseUniqueID = testCase.UniqueID;
            var testClassUniqueID = testCase.TestClass?.UniqueID;
            var testMethodUniqueID = testCase.TestMethod?.UniqueID;

            return new TestCaseStarting()
            {
                AssemblyUniqueID = assemblyUniqueID,
                Explicit = testCase.Explicit,
                SkipReason = testCase.SkipReason,
                SourceFilePath = testCase.SourceFilePath,
                SourceLineNumber = testCase.SourceLineNumber,
                TestCaseDisplayName = testCase.TestCaseDisplayName,
                TestCaseUniqueID = testCaseUniqueID,
                TestClassMetadataToken = testCase.TestClassMetadataToken,
                TestClassName = testCase.TestClassName,
                TestClassNamespace = testCase.TestClassNamespace,
                TestClassSimpleName = testCase.TestClassSimpleName,
                TestClassUniqueID = testClassUniqueID,
                TestCollectionUniqueID = testCollectionUniqueID,
                TestMethodArity = testCase.TestMethodArity,
                TestMethodMetadataToken = testCase.TestMethodMetadataToken,
                TestMethodName = testCase.TestMethod?.MethodName,
                TestMethodParameterTypesVSTest = testCase.TestMethodParameterTypesVSTest,
                TestMethodReturnTypeVSTest = testCase.TestMethodReturnTypeVSTest,
                TestMethodUniqueID = testMethodUniqueID,
                Traits = testCase.Traits,
            };
        }

        public static TestCaseFinished ToTestCaseFinished(IRetryableTestCase testCase, RunSummary summary, DateTimeOffset start)
        {
            var assemblyUniqueID = testCase.TestCollection.TestAssembly.UniqueID;
            var testCollectionUniqueID = testCase.TestCollection.UniqueID;
            var testCaseUniqueID = testCase.UniqueID;
            var testClassUniqueID = testCase.TestClass?.UniqueID;
            var testMethodUniqueID = testCase.TestMethod?.UniqueID;
            var executionTime = (decimal) (DateTimeOffset.UtcNow - start).TotalSeconds;

            return new TestCaseFinished
            {
                AssemblyUniqueID = assemblyUniqueID,
                ExecutionTime = executionTime,
                TestCaseUniqueID = testCaseUniqueID,
                TestClassUniqueID = testClassUniqueID,
                TestCollectionUniqueID = testCollectionUniqueID,
                TestMethodUniqueID = testMethodUniqueID,
                TestsFailed = summary.Failed,
                TestsNotRun = summary.NotRun,
                TestsSkipped = summary.Skipped,
                TestsTotal = summary.Total,
            };
        }
    }
}
