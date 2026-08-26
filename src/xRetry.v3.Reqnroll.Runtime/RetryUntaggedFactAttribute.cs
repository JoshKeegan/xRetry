using System;
using Xunit.v3;

namespace xRetry.v3.Reqnroll;

/// <summary>
/// Applies the project retry defaults when <c>retryUntaggedScenarios</c> is enabled;
/// otherwise, runs the generated Reqnroll scenario once.
/// </summary>
[XunitTestCaseDiscoverer(typeof(RetryFactDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public sealed class RetryUntaggedFactAttribute : RetryFactAttribute
{
    public RetryUntaggedFactAttribute()
    {
        if (!RetryDefaults.RetryUntaggedScenarios)
        {
            MaxRetries = 1;
        }
    }
}
