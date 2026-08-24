using System;
using System.Collections.Generic;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    internal static class RetryConfigurationDiscovery
    {
        /// <summary>
        /// Returns an error test case when the retry configuration is invalid, allowing discovery to exit early and
        /// report a clear configuration error.
        /// </summary>
        public static bool TryGetErrorTestCases(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute,
            out IReadOnlyCollection<IXunitTestCase> testCases)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string? error = RetryDefaults.Load(directory).Error;
            if (error is null)
            {
                testCases = null!;
                return false;
            }

            var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);
            testCases =
            [
                new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    error)
            ];
            return true;
        }
    }
}
