using System;
using Xunit.Sdk;

namespace xRetry.SpecFlow
{
    /// <summary>
    /// Applies the project retry defaults when <c>retryUntaggedScenarios</c> is enabled;
    /// otherwise, runs the generated SpecFlow scenario once.
    /// </summary>
    [XunitTestCaseDiscoverer("xRetry.RetryFactDiscoverer", "xRetry")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RetryUntaggedFactAttribute : RetryFactAttribute
    {
        public RetryUntaggedFactAttribute(params Type[] skipOnExceptions)
            : base(skipOnExceptions)
        {
            if (!RetryDefaults.Load(AppDomain.CurrentDomain.BaseDirectory).RetryUntaggedScenarios)
            {
                MaxRetries = 1;
            }
        }
    }
}
