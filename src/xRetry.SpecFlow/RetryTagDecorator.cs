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
    public class RetryTagDecorator : ITestMethodTagDecorator
    {
        private const string IGNORE_TAG = "ignore";
        private const string FACT_ATTRIBUTE = "Xunit.SkippableFactAttribute"; // TODO: These could be accessed via a custom wrapper around the XUnit2TestGeneratorProvider
        private const string THEORY_ATTRIBUTE = "Xunit.SkippableTheoryAttribute";
        private const string RETRY_FACT_ATTRIBUTE = "xRetry.RetryFact";
        private const string RETRY_THEORY_ATTRIBUTE = "xRetry.RetryTheory";

        public bool RemoveProcessedTags => false;

        public bool ApplyOtherDecoratorsForProcessedTags => true;

        private readonly IRetryTagParser retryTagParser;
        private readonly CodeDomHelper codeDomHelper;

        public int Priority => PriorityValues.Lowest;

        public RetryTagDecorator(IRetryTagParser retryTagParser, CodeDomHelper codeDomHelper)
        {
            this.retryTagParser = retryTagParser;
            this.codeDomHelper = codeDomHelper;
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            // Note: the skip check is onbly checking feature tags, as I don't know of a way to access all tags for this scenario here
            // TODO: Can be reworked
            List<string> tags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToList();

            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            //  This allows for the new (for SpecFlow 3.1.x) implementation that relies on Xunit.SkippableFact to still work, as it
            //  too will replace the attribute for running the test with a custom one.
            if (tags.Any(isIgnoreTag))
            {
                return false;
            }

            return isRetryTag(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            // TODO: Could already have been skipped if another tag on this scenario was a skip tag
            //  Handle of off attribs..?
            //  Or a later tag could be a skip tag, handle off of priorities?

            RetryTag retryTag = retryTagParser.Parse(tagName);

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

        private static bool isRetryTag(string tag) =>
            tag.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
            // Is just "retry", or is "retry("... for params
            (tag.Length == Constants.RETRY_TAG.Length || tag[Constants.RETRY_TAG.Length] == '(');
    }
}