using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using xRetry.SpecFlow.Parsers;

namespace xRetry.SpecFlow
{
    public class TestGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private const string IGNORE_TAG = "ignore";

        private readonly IRetryTagParser retryTagParser;

        public TestGeneratorProvider(CodeDomHelper codeDomHelper, ProjectSettings projectSettings, IRetryTagParser retryTagParser)
            : base(codeDomHelper, projectSettings)
        {
            this.retryTagParser = retryTagParser;
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            //  This allows for the new (for SpecFlow 3.1.x) implementation that relies on Xunit.SkippableFact to still work, as it
            //  too will replace the attribute for running the test with a custom one.
            if (scenarioCategories.Any(c => c.Equals(IGNORE_TAG, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            string strRetryTag = getRetryTag(scenarioCategories);
            if (strRetryTag == null)
            {
                return;
            }

            RetryTag retryTag = retryTagParser.Parse(strRetryTag);

            // Remove the Fact attribute
            CodeAttributeDeclaration factAttribute = testMethod.CustomAttributes
                .OfType<CodeAttributeDeclaration>().FirstOrDefault(a => a.Name == FACT_ATTRIBUTE);
            if (factAttribute != null)
            {
                testMethod.CustomAttributes.Remove(factAttribute);
            }

            // Add the Retry attribute
            CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                "xRetry.RetryFact");

            if (retryTag.MaxRetries != null)
            {
                retryAttribute.Arguments.Add(
                    new CodeAttributeArgument(new CodePrimitiveExpression(retryTag.MaxRetries)));

                if(retryTag.DelayBetweenRetriesMs != null)
                {
                    retryAttribute.Arguments.Add(
                        new CodeAttributeArgument(new CodePrimitiveExpression(retryTag.DelayBetweenRetriesMs)));
                }
            }

            // Copy arguments from the fact attribute (if there was one)
            if (factAttribute != null)
            {
                for (int i = 0; i < factAttribute.Arguments.Count; i++)
                {
                    retryAttribute.Arguments.Add(factAttribute.Arguments[i]);
                }
            }
        }

        private string getRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t =>
                t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                // Is just "retry", or is "retry("... for params
                (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
    }
}
