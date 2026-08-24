using System;
using Xunit.v3;

namespace xRetry.v3.Reqnroll;

/// <summary>
/// Applies the project retry defaults when <c>retryUntaggedScenarios</c> is enabled;
/// otherwise, runs the generated Reqnroll scenario outline once.
/// </summary>
[XunitTestCaseDiscoverer(typeof(RetryTheoryDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public sealed class RetryUntaggedTheoryAttribute : RetryTheoryAttribute
{
    public RetryUntaggedTheoryAttribute()
    {
        if (!RetryDefaults.RetryUntaggedScenarios)
        {
            MaxRetries = 1;
        }
    }
}
