using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    public class RetryTheoryDiscoverer : TheoryDiscoverer
    {
        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute,
            ITheoryDataRow dataRow,
            object?[] testMethodArguments)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(
                discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments);

            IXunitTestCase testCase;

            if (!testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "[RetryTheory] methods must have parameters. Did you mean to use [RetryFact]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "[RetryTheory] methods are not allowed to be generic.");
            }
            else if (theoryAttribute is not RetryTheoryAttribute retryTheoryAttribute)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "RetryTheoryDiscoverer only supports RetryTheoryAttribute.");
            }
            else
            {
                testCase = new RetryTestCase(
                    retryTheoryAttribute.MaxRetries,
                    retryTheoryAttribute.DelayBetweenRetriesMs,
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    details.SkipExceptions ?? [],
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    TestIntrospectionHelper.GetTraits(testMethod, dataRow),
                    testMethodArguments,
                    timeout: details.Timeout);
            }

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>([testCase]);
        }

        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);
            IXunitTestCase testCase;

            if (!testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "[RetryTheory] methods must have parameters. Did you mean to use [RetryFact]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "[RetryTheory] methods are not allowed to be generic.");
            }
            else if (theoryAttribute is not RetryTheoryAttribute retryTheoryAttribute)
            {
                testCase = new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    "RetryTheoryDiscoverer only supports RetryTheoryAttribute.");
            }
            else
            {
                testCase = new RetryTheoryDiscoveryAtRuntimeCase(
                    retryTheoryAttribute.MaxRetries,
                    retryTheoryAttribute.DelayBetweenRetriesMs,
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    theoryAttribute.SkipTestWithoutData,
                    details.SkipExceptions ?? [],
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
