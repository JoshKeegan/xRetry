using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry.v3
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
                    "[RetryFact] methods are not allowed to have parameters. Did you mean to use [RetryTheory]?");
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
                Type[] skipOnExceptions =
                    factAttribute.GetNamedArgument<Type[]>(nameof(RetryTheoryAttribute.SkipOnExceptions));

                testCase = new RetryTestCase(messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, maxRetries, delayBetweenRetriesMs,
                    skipOnExceptions);
            }

            return new[] { testCase };
        }
    }
}
