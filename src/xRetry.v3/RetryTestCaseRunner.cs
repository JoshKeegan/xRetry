using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    /// <summary>
    /// The test case runner for xRetry v3 tests, with retry and delay handling.
    /// xRetry equivalent of the built-in Xunit.v3.XunitTestCaseRunner.
    /// </summary>
    public class RetryTestCaseRunner
        : XunitTestCaseRunnerBase<XunitTestCaseRunnerContext, IXunitTestCase, IXunitTest>
    {
        private RetryTestCaseRunner() { }

        public static RetryTestCaseRunner Instance { get; } = new();

        /// <summary>
        /// Runs a test case with retry and delay handling.
        /// </summary>
        /// <param name="testCase">The test case that this invocation belongs to.</param>
        /// <param name="tests">The tests for the test case.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        /// <param name="displayName">The display name of the test case.</param>
        /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
        /// <param name="explicitOption">A flag to indicate how explicit tests should be treated.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <returns>
        /// Run summary information about the test that was run.
        /// The .Time property includes any retry attempts.
        /// </returns>
        /// <remarks>
        /// This entry point is used for both single-test (like RetryFactAttribute and individual data rows for
        /// RetryTheoryAttribute tests) and multi-test test cases (like RetryTheoryAttribute when pre-enumeration
        /// is disable or the theory data was not serializable).
        /// </remarks>
        public async ValueTask<RunSummary> Run(
            IXunitTestCase testCase,
            IReadOnlyCollection<IXunitTest> tests,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            string displayName,
            string? skipReason,
            ExplicitOption explicitOption,
            object?[] constructorArguments)
        {
            Guard.ArgumentNotNull(testCase);
            Guard.ArgumentNotNull(displayName);
            Guard.ArgumentNotNull(constructorArguments);

            await using var ctxt = new XunitTestCaseRunnerContext(
                testCase,
                tests,
                messageBus,
                aggregator,
                cancellationTokenSource,
                displayName,
                skipReason,
                explicitOption,
                constructorArguments);
            await ctxt.InitializeAsync();

            return await Run(ctxt);
        }

        protected override async ValueTask<RunSummary> RunTest(
            XunitTestCaseRunnerContext ctxt,
            IXunitTest test)
        {
            Guard.ArgumentNotNull(ctxt);
            Guard.ArgumentNotNull(test);

            if (ctxt.TestCase is not IRetryableTestCase retryableTestCase)
            {
                throw new ArgumentException("ctxt.TestCase must implement IRetryableTestCase");
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 1; ; i++)
            {
                // Prevent messages from the test run from being passed through, as we don't want 
                //  a message to mark the test as failed when we're going to retry it
                using BlockingMessageBus blockingMessageBus = new BlockingMessageBus(ctxt.MessageBus);

                ctxt.MessageBus.QueueMessage(new DiagnosticMessage(
                    "Running test \"{0}\" attempt ({1}/{2})",
                    retryableTestCase.TestCaseDisplayName,
                    i,
                    retryableTestCase.MaxRetries));

                RunSummary summary = await XunitTestRunner.Instance.Run(
                    test,
                    blockingMessageBus,
                    ctxt.ConstructorArguments,
                    ctxt.ExplicitOption,
                    ctxt.Aggregator.Clone(),
                    ctxt.CancellationTokenSource,
                    ctxt.BeforeAfterTestAttributes);

                if (summary.Failed == 0 || i == retryableTestCase.MaxRetries)
                {
                    // If we have failed (after all retries, log that)
                    if (summary.Failed != 0)
                    {
                        ctxt.MessageBus.QueueMessage(new DiagnosticMessage(
                            "Test \"{0}\" has failed and been retried the maximum number of times ({1})",
                            retryableTestCase.TestCaseDisplayName,
                            retryableTestCase.MaxRetries));
                    }

                    blockingMessageBus.Flush();
                    summary.Time = (decimal) stopwatch.Elapsed.TotalSeconds;
                    return summary;
                }
                // Otherwise log that we've had a failed run and will retry
                ctxt.MessageBus.QueueMessage(new DiagnosticMessage(
                    "Test \"{0}\" failed but is set to retry ({1}/{2}) . . .",
                    retryableTestCase.TestCaseDisplayName,
                    i,
                    retryableTestCase.MaxRetries));

                // If there is a delay between test attempts, apply it now
                if (retryableTestCase.DelayBetweenRetriesMs > 0)
                {
                    ctxt.MessageBus.QueueMessage(new DiagnosticMessage(
                        "Test \"{0}\" attempt ({1}/{2}) delayed by {3}ms. Waiting . . .",
                        retryableTestCase.TestCaseDisplayName,
                        i,
                        retryableTestCase.MaxRetries,
                        retryableTestCase.DelayBetweenRetriesMs));

                    // Don't await to prevent thread hopping.
                    //  If all of a users test cases in a collection/class are synchronous and expecting to not thread-hop
                    //  (because they're making use of thread static/thread local/managed thread ID to share data between tests rather than
                    //  a more modern async-friendly mechanism) then if a thread-hop were to happen here we'd get flickering tests.
                    //  SpecFlow relies on this as they use the managed thread ID to separate instances of some of their internal classes, which caused
                    //  a this problem for xRetry.SpecFlow: https://github.com/JoshKeegan/xRetry/issues/18
                    Task.Delay(retryableTestCase.DelayBetweenRetriesMs, ctxt.CancellationTokenSource.Token).Wait();
                }
            }
        }
    }
}
