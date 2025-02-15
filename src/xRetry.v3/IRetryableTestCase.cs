using Xunit.Sdk;

namespace xRetry.v3
{
    public interface IRetryableTestCase : IXunitTestCase
    {
        int MaxRetries { get; }
        int DelayBetweenRetriesMs { get; }
        string[] SkipOnExceptionFullNames { get; }
    }
}
