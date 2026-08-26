using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    internal static class RetryConfigurationDiscovery
    {
        /// <summary>
        /// Returns an error test case when the retry configuration is invalid, allowing discovery to exit early and
        /// report a clear configuration error.
        /// </summary>
        public static bool TryGetErrorTestCases(
            IMessageSink messageSink,
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            out IEnumerable<IXunitTestCase> testCases)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string error = RetryDefaults.Load(directory).Error;
            if (error == null)
            {
                testCases = null;
                return false;
            }

            testCases = new[]
            {
                new ExecutionErrorTestCase(
                    messageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod,
                    error)
            };
            return true;
        }
    }
}
