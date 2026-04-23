using System;
using System.Linq;
using Xunit;
using Xunit.v3;

namespace xRetry.v3
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run
    /// by the test runner up to <see cref="MaxRetries"/> times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer(typeof(RetryFactDiscoverer))]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryFactAttribute : FactAttribute
    {
        public const int DEFAULT_MAX_RETRIES = 3;
        public const int DEFAULT_DELAY_BETWEEN_RETRIES_MS = 0;

        private readonly int defaultMaxRetries;
        private readonly int defaultDelayBetweenRetriesMs;
        private int? maxRetries;
        private int? delayBetweenRetriesMs;

        public int MaxRetries
        {
            get => maxRetries ?? defaultMaxRetries;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxRetries) + " must be >= 1");
                }
                maxRetries = value;
            }
        }

        public int DelayBetweenRetriesMs
        {
            get => delayBetweenRetriesMs ?? defaultDelayBetweenRetriesMs;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(DelayBetweenRetriesMs) + " must be >= 0");
                }
                delayBetweenRetriesMs = value;
            }
        }

        /// <summary>
        /// Ctor (defaults from project config when present)
        /// </summary>
        public RetryFactAttribute()
        {
            RetryDefaults retryDefaults = RetryDefaults.Load(AppDomain.CurrentDomain.BaseDirectory);
            defaultMaxRetries = retryDefaults.MaxRetries ?? DEFAULT_MAX_RETRIES;
            defaultDelayBetweenRetriesMs = retryDefaults.DelayBetweenRetriesMs ?? DEFAULT_DELAY_BETWEEN_RETRIES_MS;
        }

        /// <summary>
        /// Ctor (explicit max retries)
        /// </summary>
        /// <param name="maxRetries">The number of times to attempt to run a test for until it succeeds</param>
        public RetryFactAttribute(int maxRetries)
            : this()
        {
            MaxRetries = maxRetries;
        }

        /// <summary>
        /// Ctor (full)
        /// </summary>
        /// <param name="maxRetries">The number of times to attempt to run a test for until it succeeds</param>
        /// <param name="delayBetweenRetriesMs">The amount of time (in ms) to wait between each test run attempt</param>
        public RetryFactAttribute(
            int maxRetries,
            int delayBetweenRetriesMs)
            : this(maxRetries)
        {
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }

    }
}
