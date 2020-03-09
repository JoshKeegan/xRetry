using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    public class RetryFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink messageSink;

        public RetryFactDiscoverer(IMessageSink messageSink)
        {
            this.messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            IXunitTestCase testCase;

            if (testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                    "[RetryFact] methods are not allowed to have parameters. Did you mean to use [Theory]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                    "[RetryFact] methods are not allowed to be generic.");
            }
            else
            {
                int maxRetries = factAttribute.GetNamedArgument<int>(nameof(RetryFactAttribute.MaxRetries));
                int delayBetweenRetriesMs =
                    factAttribute.GetNamedArgument<int>(nameof(RetryFactAttribute.DelayBetweenRetriesMs));
                testCase = new RetryTestCase(messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, maxRetries, delayBetweenRetriesMs);
            }

            return new[] { testCase };
        }
    }
}
