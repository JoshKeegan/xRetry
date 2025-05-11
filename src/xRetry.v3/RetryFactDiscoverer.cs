using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    public class RetryFactDiscoverer : IXunitTestCaseDiscoverer
    {
        public ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);
            IXunitTestCase testCase;

            if (testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    "[RetryFact] methods are not allowed to have parameters. Did you mean to use [RetryTheory]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    "[RetryFact] methods are not allowed to be generic.");
            }
            else if (factAttribute is not RetryFactAttribute retryFactAttribute)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    "RetryFactDiscoverer only supports RetryFactAttribute.");
            }
            else
            {
                testCase = new RetryTestCase(
                    retryFactAttribute.MaxRetries,
                    retryFactAttribute.DelayBetweenRetriesMs,
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    // TODO: silly hack - rework everything to use xunit native skipping
                    details.SkipExceptions ?? Array.Empty<Type>().Concat(retryFactAttribute.SkipOnExceptions).ToArray(),
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                    timeout: details.Timeout);
            }

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>([testCase]);
        }
    }
}
