using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using xRetry.SpecFlow.Parsers;

namespace xRetry.SpecFlow
{
    // Called for each test method generated, regardless of whether it has tags, allowing for feature level tags to be applied.
    public class RetryDecorator : ITestMethodDecorator
    {
        private const string IGNORE_TAG = "ignore";
        private const string FACT_ATTRIBUTE = "Xunit.SkippableFactAttribute"; // TODO: These could be accessed via a custom wrapper around the XUnit2TestGeneratorProvider
        private const string THEORY_ATTRIBUTE = "Xunit.SkippableTheoryAttribute";
        private const string RETRY_FACT_ATTRIBUTE = "xRetry.RetryFact";
        private const string RETRY_THEORY_ATTRIBUTE = "xRetry.RetryTheory";

        private readonly IRetryTagParser retryTagParser;
        private readonly CodeDomHelper codeDomHelper;

        public int Priority => PriorityValues.Lowest;

        public RetryDecorator(IRetryTagParser retryTagParser, CodeDomHelper codeDomHelper)
        {
            this.retryTagParser = retryTagParser;
            this.codeDomHelper = codeDomHelper;
        }

        public bool CanDecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod) =>
            getRetryTag(generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name))) != null;

        public void DecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            List<string> tags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToList();

            // SpecFlow xUnit handles ignore tags on the feature as a special case (XUnit2TestGeneratorProvider.SetTestClassIgnore).
            //  I'm not clear why there are different mechanisms in the code being generated for this (probably due to compatibility
            //  with nunit & mstest, but could be tech debt in SpecFlow).
            // Anyway, do not modify scenarios that are part of an ignored feature.
            if (tags.Any(isIgnoreTag))
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
            CodeAttributeDeclaration retryAttribute = codeDomHelper.AddAttribute(testMethod,
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
