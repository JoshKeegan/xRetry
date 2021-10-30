using xRetry.Exceptions;

namespace xRetry
{
    public static class Skip
    {
        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="reason">Reason for the test needing to be skipped</param>
        public static void Always(string reason = null)
        {
            throw new SkipTestException(reason);
        }
    }
}
