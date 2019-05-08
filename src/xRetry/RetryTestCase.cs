using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    [Serializable]
    public class RetryTestCase : XunitTestCase
    {
        private int maxRetries;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes", true)]
        public RetryTestCase() { }

        public RetryTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            int maxRetries,
            object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod,
                testMethodArguments)
        {
            this.maxRetries = maxRetries;
        }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
            object[] constructorArguments, ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            for (int i = 1; ; i++)
            {
                // Prevent messages from the test run from being passed through, as we don't want 
                //  a message to mark the test as failed when we're going to retry it
                using (BlockingMessageBus blockingMessageBus = new BlockingMessageBus(messageBus))
                {
                    RunSummary summary = await base.RunAsync(diagnosticMessageSink, blockingMessageBus,
                        constructorArguments, aggregator, cancellationTokenSource);

                    // If we succeeded, or we've reached the max retries return the result
                    if (summary.Failed == 0 || i == maxRetries)
                    {
                        blockingMessageBus.Flush();
                        return summary;
                    }
                    // Otherwise log that we've had a failed run and will retry
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        "Test \"{0}\" failed but is set to retry ({1}/{2}), retrying . . .", DisplayName, i,
                        maxRetries));
                }
            }
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("maxRetries", maxRetries);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            maxRetries = data.GetValue<int>("maxRetries");
        }
    }
}
