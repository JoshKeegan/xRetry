using System;
using Xunit;
using Xunit.Sdk;

namespace XunitRetry
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run
    /// by the test runner up to MaxRetries times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("XunitRetry.RetryTestCaseDiscoverer", "XunitRetry")]
    public class RetryFactAttribute : FactAttribute
    {
        public readonly int MaxRetries;

        public RetryFactAttribute(int maxRetries = 3)
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries) + " must be >= 1");
            }

            MaxRetries = maxRetries;
        }
    }
}
