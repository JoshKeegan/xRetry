using Xunit.v3;

namespace xRetry.v3
{
    public interface IRetryableTestCase : ISelfExecutingXunitTestCase
    {
        int MaxRetries { get; }
        int DelayBetweenRetriesMs { get; }
    }
}
