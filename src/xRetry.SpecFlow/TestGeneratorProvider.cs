using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using xRetry.SpecFlow.Parsers;

namespace xRetry.SpecFlow
{
    public class TestGeneratorProvider : CustomXUnit2TestGeneratorProvider
    {
        private const string IGNORE_TAG = "ignore";
        private const string RETRY_FACT_ATTRIBUTE = "xRetry.RetryFact";
        private const string RETRY_THEORY_ATTRIBUTE = "xRetry.RetryTheory";

        private readonly IRetryTagParser retryTagParser;

        public TestGeneratorProvider(CodeDomHelper codeDomHelper, ProjectSettings projectSettings, IRetryTagParser retryTagParser)
            : base(codeDomHelper, projectSettings)
        {
            this.retryTagParser = retryTagParser;
        }

        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetRowTest(generationContext, testMethod, scenarioTitle);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();

            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod);
        }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            base.SetTestMethod(generationContext, testMethod, friendlyTestName);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();

            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod);
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            IEnumerable<string> featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name));
            applyRetry((string[]) scenarioCategories, featureTags, testMethod);
        }

        /// <summary>
        /// Apply retry tags to the current test
        /// </summary>
        /// <param name="tags">Tags that haven't yet been processed. If the test has just been created these will be for the feature, otherwise for the scenario</param>
        /// <param name="processedTags">Tags that have already been processed. If the test has just been created this will be empty, otherwise they will be the feature tags</param>
        /// <param name="testMethod">Test method we are applying retries for</param>
        private void applyRetry(IList<string> tags, IEnumerable<string> processedTags, CodeMemberMethod testMethod)
        {
            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            //  This allows for the new (for SpecFlow 3.1.x) implementation that relies on Xunit.SkippableFact to still work, as it
            //  too will replace the attribute for running the test with a custom one.
            if (tags.Any(isIgnoreTag) || processedTags.Any(isIgnoreTag))
            {
                return;
            }

            string strRetryTag = getRetryTag(tags);
            if (strRetryTag == null)
            {
                return;
            }

            RetryTag retryTag = retryTagParser.Parse(strRetryTag);

            // Remove the original fact or theory attribute
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a =>
                    a.Name == FACT_ATTRIBUTE || 
                    a.Name == THEORY_ATTRIBUTE || 
                    a.Name == RETRY_FACT_ATTRIBUTE ||
                    a.Name == RETRY_THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }
            testMethod.CustomAttributes.Remove(originalAttribute);

            // Add the Retry attribute
            CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                originalAttribute.Name == FACT_ATTRIBUTE || originalAttribute.Name == RETRY_FACT_ATTRIBUTE
                    ? RETRY_FACT_ATTRIBUTE
                    : RETRY_THEORY_ATTRIBUTE);

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

            // Copy arguments from the original attribute. If it's already a retry attribute, don't copy the retry arguments though
            for (int i = originalAttribute.Name == RETRY_FACT_ATTRIBUTE ||
                         originalAttribute.Name == RETRY_THEORY_ATTRIBUTE
                     ? retryAttribute.Arguments.Count
                     : 0;
                 i < originalAttribute.Arguments.Count;
                 i++)
            {
                retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
            }
        }

        private static string stripLeadingAtSign(string s) => s.StartsWith("@") ? s.Substring(1) : s;

        private static bool isIgnoreTag(string tag) => tag.Equals(IGNORE_TAG, StringComparison.OrdinalIgnoreCase);

        private static string getRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t =>
                t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                // Is just "retry", or is "retry("... for params
                (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
    }
}
