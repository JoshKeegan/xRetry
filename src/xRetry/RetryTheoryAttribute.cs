using System;
using Xunit.Sdk;

namespace xRetry
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a theory that should be run
    /// by the test runner up to MaxRetries times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("xRetry.RetryTheoryDiscoverer", "xRetry")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryTheoryAttribute : RetryFactAttribute
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="maxRetries">The number of times to run a test for until it succeeds</param>
        /// <param name="delayBetweenRetriesMs">The amount of time (in ms) to wait between each test run attempt</param>
        public RetryTheoryAttribute(int maxRetries = 3, int delayBetweenRetriesMs = 0)
            : base(maxRetries, delayBetweenRetriesMs) {  }
    }
}
