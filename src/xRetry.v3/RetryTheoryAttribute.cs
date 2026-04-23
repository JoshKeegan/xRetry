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
        public RetryTheoryAttribute() { }

        /// <inheritdoc/>
        public RetryTheoryAttribute(int maxRetries)
            : base(maxRetries) { }

        /// <inheritdoc/>
        public RetryTheoryAttribute(
            int maxRetries,
            int delayBetweenRetriesMs)
            : base(maxRetries, delayBetweenRetriesMs) { }
    }
}
