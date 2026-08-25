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

        private int? maxRetries;
        private int? delayBetweenRetriesMs;

        protected RetryDefaults RetryDefaults { get; }

        public int MaxRetries
        {
            get => maxRetries ?? RetryDefaults.MaxRetries ?? DEFAULT_MAX_RETRIES;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxRetries must be >= 1");
                }
                maxRetries = value;
            }
        }

        public int DelayBetweenRetriesMs
        {
            get => delayBetweenRetriesMs ??
                RetryDefaults.DelayBetweenRetriesMs ??
                DEFAULT_DELAY_BETWEEN_RETRIES_MS;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "DelayBetweenRetriesMs must be >= 0");
                }
                delayBetweenRetriesMs = value;
            }
        }

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

            RetryDefaults = RetryDefaults
                .Load(AppDomain.CurrentDomain.BaseDirectory)
                .Value;
        }

        /// <summary>
        /// Ctor (explicit max retries)
        /// </summary>
        /// <param name="maxRetries">The number of times to attempt to run a test for until it succeeds</param>
        /// <param name="skipOnExceptions">Mark the test as skipped when this type of exception is encountered</param>
        public RetryFactAttribute(int maxRetries, params Type[] skipOnExceptions)
            : this(skipOnExceptions)
        {
            MaxRetries = maxRetries;
        }

        /// <summary>
        /// Ctor (full)
        /// </summary>
        /// <param name="maxRetries">The number of times to attempt to run a test for until it succeeds</param>
        /// <param name="delayBetweenRetriesMs">The amount of time (in ms) to wait between each test run attempt</param>
        /// <param name="skipOnExceptions">Mark the test as skipped when this type of exception is encountered</param>
        public RetryFactAttribute(
            int maxRetries,
            int delayBetweenRetriesMs,
            params Type[] skipOnExceptions)
            : this(maxRetries, skipOnExceptions)
        {
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }
    }
}
