using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using xRetry.Exceptions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    [Serializable]
    public class RetryTestCase : XunitTestCase, IRetryableTestCase
    {
        public int MaxRetries { get; private set; }
        public int DelayBetweenRetriesMs { get; private set; }
        public string[] SkipOnExceptionFullNames { get; private set; }

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
            int delayBetweenRetriesMs,
            Type[] skipOnExceptions,
            object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod,
                testMethodArguments)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
            SkipOnExceptionFullNames = GetSkipOnExceptionFullNames(skipOnExceptions);
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
            object[] constructorArguments, ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) =>
            RetryTestCaseRunner.RunAsync(this, diagnosticMessageSink, messageBus, cancellationTokenSource,
                blockingMessageBus => new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments,
                        TestMethodArguments, blockingMessageBus, aggregator, cancellationTokenSource)
                    .RunAsync());

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("MaxRetries", MaxRetries);
            data.AddValue("DelayBetweenRetriesMs", DelayBetweenRetriesMs);
            data.AddValue("SkipOnExceptionFullNames", SkipOnExceptionFullNames);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            MaxRetries = data.GetValue<int>("MaxRetries");
            DelayBetweenRetriesMs = data.GetValue<int>("DelayBetweenRetriesMs");
            SkipOnExceptionFullNames = data.GetValue<string[]>("SkipOnExceptionFullNames");
        }

        public static string[] GetSkipOnExceptionFullNames(Type[] customSkipOnExceptions)
        {
            string[] toRet = new string[customSkipOnExceptions.Length + 1];
            for (int i = 0; i < customSkipOnExceptions.Length; i++)
            {
                toRet[i] = customSkipOnExceptions[i].FullName;
            }
            toRet[toRet.Length - 1] = typeof(SkipTestException).FullName;
            return toRet;
        }
    }
}
