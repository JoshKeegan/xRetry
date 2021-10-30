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
            if (isIgnored(generationContext, scenarioCategories))
            {
                return;
            }

            string strRetryTag = getRetryTag(scenarioCategories);
            if (strRetryTag == null)
            {
                return;
            }

            RetryTag retryTag = retryTagParser.Parse(strRetryTag);

            // Remove the original fact or theory attribute
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == FACT_ATTRIBUTE || a.Name == THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }
            testMethod.CustomAttributes.Remove(originalAttribute);

            // Add the Retry attribute
            CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                "xRetry.Retry" + (originalAttribute.Name == FACT_ATTRIBUTE ? "Fact" : "Theory"));

            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodePrimitiveExpression(retryTag.MaxRetries ?? RetryFactAttribute.DEFAULT_MAX_RETRIES)));
            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodePrimitiveExpression(retryTag.DelayBetweenRetriesMs ??
                                            RetryFactAttribute.DEFAULT_DELAY_BETWEEN_RETRIES_MS)));

            // Always skip on Xunit.SkipException (from Xunit.SkippableFact) which is used by SpecFlow.xUnit to implement
            //  dynamic test skipping. This way we can intercept the exception that is already thrown without also having
            //  our own runtime plugin.
            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodeArrayCreateExpression(new CodeTypeReference(typeof(Type)),
                    new CodeExpression[]
                    {
                        new CodeTypeOfExpression(typeof(Xunit.SkipException))
                    })));

            // Copy arguments from the original attribute
            for (int i = 0; i < originalAttribute.Arguments.Count; i++)
            {
                retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
            }
        }

        private static bool isIgnored(TestClassGenerationContext generationContext, IEnumerable<string> tags) =>
            generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).Any(isIgnoreTag) ||
            tags.Any(isIgnoreTag);

        private static string stripLeadingAtSign(string s) => s.StartsWith("@") ? s.Substring(1) : s;

        private static bool isIgnoreTag(string tag) => tag.Equals(IGNORE_TAG, StringComparison.OrdinalIgnoreCase);

        private static string getRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t =>
                t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                // Is just "retry", or is "retry("... for params
                (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
    }
}
