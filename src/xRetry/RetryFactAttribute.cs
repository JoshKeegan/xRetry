using System;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace xRetry
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run
    /// by the test runner up to <see cref="MaxRetries"/> times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("xRetry.RetryFactDiscoverer", "xRetry")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryFactAttribute : FactAttribute
    {
        public const int DEFAULT_MAX_RETRIES = 3;
        public const int DEFAULT_DELAY_BETWEEN_RETRIES_MS = 0;

        public readonly int MaxRetries = DEFAULT_MAX_RETRIES;
        public readonly int DelayBetweenRetriesMs = DEFAULT_DELAY_BETWEEN_RETRIES_MS;
        public readonly Type[] SkipOnExceptions;

        /// <summary>
        /// Ctor (just skip on exceptions)
        /// </summary>
        /// <param name="skipOnExceptions">Mark the test as skipped when this type of exception is encountered</param>
        public RetryFactAttribute(params Type[] skipOnExceptions)
        {
            SkipOnExceptions = skipOnExceptions ?? Type.EmptyTypes;

            if (SkipOnExceptions.Any(t => !t.IsSubclassOf(typeof(Exception))))
            {
                throw new ArgumentException("Specified type must be an exception", nameof(skipOnExceptions));
            }
        }

        /// <summary>
        /// Ctor (full)
        /// </summary>
        /// <param name="maxRetries">The number of times to attempt to run a test for until it succeeds</param>
        /// <param name="delayBetweenRetriesMs">The amount of time (in ms) to wait between each test run attempt</param>
        /// <param name="skipOnExceptions">Mark the test as skipped when this type of exception is encountered</param>
        public RetryFactAttribute(
            int maxRetries = DEFAULT_MAX_RETRIES,
            int delayBetweenRetriesMs = DEFAULT_DELAY_BETWEEN_RETRIES_MS,
            params Type[] skipOnExceptions)
            : this(skipOnExceptions)
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries) + " must be >= 1");
            }
            if (delayBetweenRetriesMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delayBetweenRetriesMs) + " must be >= 0");
            }

            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }
    }
}
