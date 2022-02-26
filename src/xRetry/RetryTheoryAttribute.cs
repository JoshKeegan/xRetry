using System;
using Xunit.Sdk;

namespace xRetry
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a theory that should be run
    /// by the test runner up to <see cref="RetryFactAttribute.MaxRetries"/> times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("xRetry.RetryTheoryDiscoverer", "xRetry")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryTheoryAttribute : RetryFactAttribute
    {
        /// <inheritdoc/>
        public RetryTheoryAttribute(params Type[] skipOnExceptions)
            : base(skipOnExceptions) {  }

        /// <inheritdoc/>
        public RetryTheoryAttribute(
            int maxRetries = DEFAULT_MAX_RETRIES, 
            int delayBetweenRetriesMs = DEFAULT_DELAY_BETWEEN_RETRIES_MS, 
            params Type[] skipOnExceptions)
            : base(maxRetries, delayBetweenRetriesMs, skipOnExceptions) {  }
    }
}
