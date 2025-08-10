using System;
using Xunit.v3;

namespace xRetry.v3
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a theory that should be run
    /// by the test runner up to <see cref="RetryFactAttribute.MaxRetries"/> times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer(typeof(RetryTheoryDiscoverer))]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryTheoryAttribute : RetryFactAttribute, ITheoryAttribute
    {
        public bool DisableDiscoveryEnumeration { get; set; }
        public bool SkipTestWithoutData { get; set; }

        /// <inheritdoc/>
        public RetryTheoryAttribute(
            int maxRetries = DEFAULT_MAX_RETRIES,
            int delayBetweenRetriesMs = DEFAULT_DELAY_BETWEEN_RETRIES_MS)
            : base(maxRetries, delayBetweenRetriesMs) { }
    }
}
