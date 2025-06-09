using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    [Serializable]
    public class RetryTestCase : XunitTestCase, IRetryableTestCase
    {
        public int MaxRetries { get; private set; }
        public int DelayBetweenRetriesMs { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes",
            true)]
        public RetryTestCase() {  }

        public RetryTestCase(
            int maxRetries,
            int delayBetweenRetriesMs,
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueId,
            bool @explicit,
            Type[] skipExceptions,
            string? skipReason = null,
            Type? skipType = null,
            string? skipUnless = null,
            string? skipWhen = null,
            Dictionary<string, HashSet<string>>? traits = null,
            object?[]? testMethodArguments = null,
            string? sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(testMethod, testCaseDisplayName, uniqueId, @explicit, skipExceptions, skipReason, skipType,
                skipUnless, skipWhen, traits, testMethodArguments, sourceFilePath, sourceLineNumber, timeout)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }

        /// <inheritdoc />
        public ValueTask<RunSummary> Run(
            ExplicitOption explicitOption,
            IMessageBus messageBus,
            object?[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) =>
            RetryTestCaseRunner.Run(
                this,
                messageBus,
                cancellationTokenSource,
                async blockingMessageBus => await XunitTestRunner.Instance.Run(
                    (await CreateTests()).First(), // Can only be one test in XunitTestCase.
                    blockingMessageBus,
                    constructorArguments,
                    explicitOption,
                    aggregator.Clone(),
                    cancellationTokenSource,
                    TestMethod.BeforeAfterTestAttributes));

        protected override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("MaxRetries", MaxRetries);
            data.AddValue("DelayBetweenRetriesMs", DelayBetweenRetriesMs);
        }

        protected override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            MaxRetries = data.GetValue<int>("MaxRetries");
            DelayBetweenRetriesMs = data.GetValue<int>("DelayBetweenRetriesMs");
        }
    }
}
