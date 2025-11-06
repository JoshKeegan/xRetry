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

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public RetryTheoryDiscoveryAtRuntimeCase() { }

        public RetryTheoryDiscoveryAtRuntimeCase(
            int maxRetries,
            int delayBetweenRetriesMs,
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueId,
            bool @explicit,
            bool skipTestWithoutData,
            Type[] skipExceptions,
            string? skipReason = null,
            Type? skipType = null,
            string? skipUnless = null,
            string? skipWhen = null,
            Dictionary<string, HashSet<string>>? traits = null,
            string? sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(testMethod, testCaseDisplayName, uniqueId, @explicit, skipTestWithoutData, skipExceptions,
                skipReason, skipType, skipUnless, skipWhen, traits, sourceFilePath, sourceLineNumber, timeout)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }

        /// <inheritdoc />
        public async ValueTask<RunSummary> Run(
            ExplicitOption explicitOption,
            IMessageBus messageBus,
            object?[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            var overall = new RunSummary();

            var tests = await CreateTests();
            foreach (var test in tests)
            {
                var single = await RetryTestCaseRunner.Run(
                    this,
                    messageBus,
                    cancellationTokenSource,
                    async blockingMessageBus => await XunitTestRunner.Instance.Run(
                        test,
                        blockingMessageBus,
                        constructorArguments,
                        explicitOption,
                        aggregator.Clone(),
                        cancellationTokenSource,
                        TestMethod.BeforeAfterTestAttributes));

                overall.Aggregate(single);
            }

            return overall;
        }

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
