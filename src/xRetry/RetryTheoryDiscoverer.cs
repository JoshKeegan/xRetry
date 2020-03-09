using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    public class RetryTheoryDiscoverer : TheoryDiscoverer
    {
        public RetryTheoryDiscoverer(IMessageSink diagnosticMessageSink) 
            : base(diagnosticMessageSink) {  }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute,
            object[] dataRow)
        {
            int maxRetries = theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.MaxRetries));
            int delayBetweenRetriesMs =
                theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.DelayBetweenRetriesMs));
            return new[]
            {
                new RetryTestCase(
                    DiagnosticMessageSink, 
                    discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), 
                    testMethod, 
                    maxRetries, 
                    delayBetweenRetriesMs,
                    dataRow)
            };
        }

        // TODO: See https://github.com/xunit/xunit/blob/3f3f6929c301db40e9d5d8ced2bb494084f57c82/src/xunit.execution/Sdk/Frameworks/TheoryDiscoverer.cs#L75
        //  Need to think this through and come up with test cases to cover this
        //protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        //{
        //    return base.CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        //}
    }
}
