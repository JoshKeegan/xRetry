using Xunit.Sdk;

namespace xRetry
{
    public interface IRetryableTestCase : IXunitTestCase
    {
        int MaxRetries { get; }
        int DelayBetweenRetriesMs { get; }
    }
}
