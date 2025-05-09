using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    /// <summary>
    /// Represents a test case to be retried which runs multiple tests for theory data, either because the
    /// data was not enumerable or because the data was not serializable.
    /// Equivalent to xunit's XunitTheoryTestCase 
    /// </summary>
    [Serializable]
    public class RetryTheoryDiscoveryAtRuntimeCase : XunitDelayEnumeratedTheoryTestCase, IRetryableTestCase
    {
        public int MaxRetries { get; private set; }
        public int DelayBetweenRetriesMs { get; private set; }
        public string[] SkipOnExceptionFullNames { get; private set; }

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public RetryTheoryDiscoveryAtRuntimeCase()
        {
            SkipOnExceptionFullNames = [];
        }

        public RetryTheoryDiscoveryAtRuntimeCase(
            int maxRetries,
            int delayBetweenRetriesMs,
            Type[] skipOnExceptions,
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueId,
            bool @explicit,
            bool skipTestWithoutData,
            string? skipReason = null,
            Type? skipType = null,
            string? skipUnless = null,
            string? skipWhen = null,
            Dictionary<string, HashSet<string>>? traits = null,
            string? sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(testMethod, testCaseDisplayName, uniqueId, @explicit, skipTestWithoutData, skipReason, skipType,
                skipUnless, skipWhen, traits, sourceFilePath, sourceLineNumber, timeout)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
            SkipOnExceptionFullNames = RetryTestCase.GetSkipOnExceptionFullNames(skipOnExceptions);
        }

        // TODO: needs rethinking - the way I have this in RetryTestCase wouldn't work here as that assumes a single
        // test, but XunitDelayEnumeratedTheoryTestCase.CreateTests can produce multiple
        /// <inheritdoc />
        public ValueTask<RunSummary> Run(ExplicitOption explicitOption, IMessageBus messageBus, object?[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) =>
            throw new NotImplementedException();

        // TODO: delete old
        // public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
        //     object[] constructorArguments, ExceptionAggregator aggregator,
        //     CancellationTokenSource cancellationTokenSource) =>
        //     RetryTestCaseRunner.Run(this, diagnosticMessageSink, messageBus, cancellationTokenSource,
        //         blockingMessageBus => new XunitTheoryTestCaseRunner(this, DisplayName, SkipReason, constructorArguments,
        //                 diagnosticMessageSink, blockingMessageBus, aggregator, cancellationTokenSource)
        //             .RunAsync());

        protected override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("MaxRetries", MaxRetries);
            data.AddValue("DelayBetweenRetriesMs", DelayBetweenRetriesMs);
            data.AddValue("SkipOnExceptionFullNames", SkipOnExceptionFullNames);
        }

        protected override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            MaxRetries = data.GetValue<int>("MaxRetries");
            DelayBetweenRetriesMs = data.GetValue<int>("DelayBetweenRetriesMs");
            SkipOnExceptionFullNames = data.GetValue<string[]>("SkipOnExceptionFullNames") ?? [];
        }
    }
}
