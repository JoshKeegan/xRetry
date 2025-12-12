using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Reqnroll.Generator;
using Reqnroll.Generator.CodeDom;
using Reqnroll.Generator.UnitTestProvider;
using xRetry.Reqnroll.v3.Parsers;
using xRetry.v3;

namespace xRetry.Reqnroll.v3
{
    /// <summary>
    /// A decorator for IUnitTestGeneratorProvider that adds retry functionality to xUnit v3 tests.
    /// This wraps the actual generator provider (XUnit3TestGeneratorProvider from Reqnroll.xunit.v3)
    /// and modifies test attributes to use RetryFact/RetryTheory when @retry tags are present.
    /// </summary>
    public class TestGeneratorProvider : IUnitTestGeneratorProvider
    {
        private const string IGNORE_TAG = "ignore";

        // xUnit v3 attributes (used by XUnit3TestGeneratorProvider)
        private const string XUNIT3_FACT_ATTRIBUTE = "Xunit.FactAttribute";
        private const string XUNIT3_THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";

        // xRetry.v3 attributes
        private const string RETRY_FACT_ATTRIBUTE = "xRetry.v3.RetryFact";
        private const string RETRY_THEORY_ATTRIBUTE = "xRetry.v3.RetryTheory";

        private readonly IUnitTestGeneratorProvider innerProvider;
        private readonly IRetryTagParser retryTagParser;
        private readonly CodeDomHelper codeDomHelper;

        public TestGeneratorProvider(CodeDomHelper codeDomHelper, IRetryTagParser retryTagParser, IUnitTestGeneratorProvider innerProvider)
        {
            this.codeDomHelper = codeDomHelper;
            this.retryTagParser = retryTagParser;
            this.innerProvider = innerProvider;
        }

        // Delegate all interface methods to inner provider, but intercept the ones that set test attributes

        public UnitTestGeneratorTraits GetTraits() => innerProvider.GetTraits();

        public void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
            => innerProvider.SetTestClass(generationContext, featureTitle, featureDescription);

        public void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
            => innerProvider.SetTestClassCategories(generationContext, featureCategories);

        public void SetTestClassIgnore(TestClassGenerationContext generationContext)
            => innerProvider.SetTestClassIgnore(generationContext);

        public void FinalizeTestClass(TestClassGenerationContext generationContext)
            => innerProvider.FinalizeTestClass(generationContext);

        public void SetTestClassNonParallelizable(TestClassGenerationContext generationContext)
            => innerProvider.SetTestClassNonParallelizable(generationContext);

        public void SetTestMethodNonParallelizable(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            => innerProvider.SetTestMethodNonParallelizable(generationContext, testMethod);

        public void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
            => innerProvider.SetTestClassInitializeMethod(generationContext);

        public void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
            => innerProvider.SetTestClassCleanupMethod(generationContext);

        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
            => innerProvider.SetTestInitializeMethod(generationContext);

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
            => innerProvider.SetTestCleanupMethod(generationContext);

        public void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            => innerProvider.SetTestMethodIgnore(generationContext, testMethod);

        // Called for scenario outlines
        public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            innerProvider.SetRowTest(generationContext, testMethod, scenarioTitle);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();
            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod);
        }

        public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
            => innerProvider.SetRow(generationContext, testMethod, arguments, tags, isIgnored);

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
            => innerProvider.SetTestMethodAsRow(generationContext, testMethod, scenarioTitle, exampleSetName, variantName, arguments);

        // Called for scenarios
        public void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            innerProvider.SetTestMethod(generationContext, testMethod, friendlyTestName);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();
            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod);
        }

        // Called for both scenarios & scenario outlines, but only if it has tags
        public void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            innerProvider.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Feature tags will have already been processed in one of the methods above, which are executed before this
            IEnumerable<string> featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name));
            applyRetry((string[]) scenarioCategories, featureTags, testMethod);
        }

        public void MarkCodeMethodInvokeExpressionAsAwait(CodeMethodInvokeExpression expression)
            => innerProvider.MarkCodeMethodInvokeExpressionAsAwait(expression);

        /// <summary>
        /// Apply retry tags to the current test
        /// </summary>
        /// <param name="tags">Tags that haven't yet been processed. If the test has just been created these will be for the feature, otherwise for the scenario</param>
        /// <param name="processedTags">Tags that have already been processed. If the test has just been created this will be empty, otherwise they will be the feature tags</param>
        /// <param name="testMethod">Test method we are applying retries for</param>
        private void applyRetry(IList<string> tags, IEnumerable<string> processedTags, CodeMemberMethod testMethod)
        {
            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
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

            // Remove the original fact or theory attribute from xUnit v3
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a =>
                    a.Name == XUNIT3_FACT_ATTRIBUTE ||
                    a.Name == XUNIT3_THEORY_ATTRIBUTE ||
                    a.Name == RETRY_FACT_ATTRIBUTE ||
                    a.Name == RETRY_THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }
            testMethod.CustomAttributes.Remove(originalAttribute);

            // Determine if this is a theory (scenario outline) or fact (scenario)
            bool isTheory = originalAttribute.Name == XUNIT3_THEORY_ATTRIBUTE ||
                           originalAttribute.Name == RETRY_THEORY_ATTRIBUTE;

            // Add the Retry attribute
            CodeAttributeDeclaration retryAttribute = codeDomHelper.AddAttribute(testMethod,
                isTheory ? RETRY_THEORY_ATTRIBUTE : RETRY_FACT_ATTRIBUTE);

            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodePrimitiveExpression(retryTag.MaxRetries ?? RetryFactAttribute.DEFAULT_MAX_RETRIES)));
            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodePrimitiveExpression(retryTag.DelayBetweenRetriesMs ??
                                            RetryFactAttribute.DEFAULT_DELAY_BETWEEN_RETRIES_MS)));

            // Copy arguments from the original attribute (like DisplayName)
            // If it's already a retry attribute, don't copy the retry arguments though
            bool isOriginallyRetryAttribute = originalAttribute.Name == RETRY_FACT_ATTRIBUTE ||
                                              originalAttribute.Name == RETRY_THEORY_ATTRIBUTE;
            for (int i = isOriginallyRetryAttribute ? retryAttribute.Arguments.Count : 0;
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
