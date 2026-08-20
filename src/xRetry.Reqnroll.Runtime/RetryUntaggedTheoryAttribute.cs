using System;
using Xunit.Sdk;

namespace xRetry.Reqnroll
{
    /// <summary>
    /// Applies the project retry defaults when <c>retryUntaggedScenarios</c> is enabled;
    /// otherwise, runs the generated Reqnroll scenario outline once.
    /// </summary>
    [XunitTestCaseDiscoverer("xRetry.RetryTheoryDiscoverer", "xRetry")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RetryUntaggedTheoryAttribute : RetryTheoryAttribute
    {
        public RetryUntaggedTheoryAttribute(params Type[] skipOnExceptions)
            : base(skipOnExceptions)
        {
            if (!RetryDefaults.Load(AppDomain.CurrentDomain.BaseDirectory).RetryUntaggedScenarios)
            {
                MaxRetries = 1;
            }
        }
    }
}
