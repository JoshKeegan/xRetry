using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Reqnroll.Generator;
using Reqnroll.Generator.CodeDom;
using Reqnroll.Generator.Interfaces;
using Reqnroll.Generator.UnitTestProvider;
using xRetry.Reqnroll.Parsers;

namespace xRetry.Reqnroll
{
    public class TestGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private const string IGNORE_TAG = "ignore";
        private const string RETRY_FACT_ATTRIBUTE = "xRetry.RetryFact";
        private const string RETRY_THEORY_ATTRIBUTE = "xRetry.RetryTheory";
        private const string RETRY_UNTAGGED_FACT_ATTRIBUTE = "xRetry.Reqnroll.RetryUntaggedFact";
        private const string RETRY_UNTAGGED_THEORY_ATTRIBUTE = "xRetry.Reqnroll.RetryUntaggedTheory";

        private readonly IRetryTagParser retryTagParser;

        public TestGeneratorProvider(CodeDomHelper codeDomHelper, IRetryTagParser retryTagParser)
            : base(codeDomHelper)
        {
            this.retryTagParser = retryTagParser;
        }

        // Called for scenario outlines, even when it has no tags.
        // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetRowTest(generationContext, testMethod, scenarioTitle);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();

            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod, addRetryUntaggedAttribute: true);
        }

        // Called for scenarios, even when it has no tags.
        // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            base.SetTestMethod(generationContext, testMethod, friendlyTestName);

            string[] featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();

            applyRetry(featureTags, Enumerable.Empty<string>(), testMethod, addRetryUntaggedAttribute: true);
        }

        // Called for both scenarios & scenario outlines, but only if it has tags
        public override void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Feature tags will have already been processed in one of the methods above, which are executed before this
            IEnumerable<string> featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name));
            applyRetry((string[]) scenarioCategories, featureTags, testMethod, addRetryUntaggedAttribute: false);
        }

        // @ignore is handled after the first retry pass, so Fact/Theory may already have been
        // changed to a retry-untagged or explicit retry attribute. Restore the xUnit attribute
        // before delegating so the underlying provider can set Skip; the later retry pass
        // sees Skip and leaves the ignored test un-retried.
        public override void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            revertRetryAttribute(testMethod);
            base.SetTestMethodIgnore(generationContext, testMethod);
        }

        /// <summary>
        /// Apply retry tags to the current test
        /// </summary>
        /// <param name="tags">Tags that haven't yet been processed. If the test has just been created these will be for the feature, otherwise for the scenario</param>
        /// <param name="processedTags">Tags that have already been processed. If the test has just been created this will be empty, otherwise they will be the feature tags</param>
        /// <param name="testMethod">Test method we are applying retries for</param>
        /// <param name="addRetryUntaggedAttribute">Whether to add the runtime retry-untagged attribute to the scenario</param>
        private void applyRetry(
            IList<string> tags,
            IEnumerable<string> processedTags,
            CodeMemberMethod testMethod,
            bool addRetryUntaggedAttribute)
        {
            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            //  This allows for the new (for Reqnroll 3.1.x) implementation that relies on Xunit.SkippableFact to still work, as it
            //  too will replace the attribute for running the test with a custom one.
            if (isTestMethodAlreadyIgnored(testMethod) || tags.Any(isIgnoreTag) || processedTags.Any(isIgnoreTag))
            {
                return;
            }

            if (addRetryUntaggedAttribute)
            {
                replaceWithRetryUntaggedAttribute(testMethod);
            }

            string strRetryTag = getRetryTag(tags);
            if (strRetryTag == null)
            {
                return;
            }

            RetryTag retryTag = retryTagParser.Parse(strRetryTag);
            replaceWithRetryAttribute(testMethod, retryTag.MaxRetries, retryTag.DelayBetweenRetriesMs);
        }

        private void replaceWithRetryUntaggedAttribute(CodeMemberMethod testMethod)
        {
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == FACT_ATTRIBUTE || a.Name == THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }

            testMethod.CustomAttributes.Remove(originalAttribute);

            CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                originalAttribute.Name == FACT_ATTRIBUTE
                    ? RETRY_UNTAGGED_FACT_ATTRIBUTE
                    : RETRY_UNTAGGED_THEORY_ATTRIBUTE);

            addSkipOnExceptionArgument(retryAttribute);

            foreach (CodeAttributeArgument argument in originalAttribute.Arguments)
            {
                retryAttribute.Arguments.Add(argument);
            }
        }

        private void replaceWithRetryAttribute(
            CodeMemberMethod testMethod,
            int? maxRetries,
            int? delayBetweenRetriesMs)
        {
            // Remove the original fact or theory attribute
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a =>
                    a.Name == FACT_ATTRIBUTE ||
                    a.Name == THEORY_ATTRIBUTE ||
                    a.Name == RETRY_FACT_ATTRIBUTE ||
                    a.Name == RETRY_THEORY_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }

            RetryAttributeArguments existingRetryArguments = getExistingRetryArguments(originalAttribute);
            maxRetries = maxRetries ?? existingRetryArguments.MaxRetries;
            delayBetweenRetriesMs = delayBetweenRetriesMs ?? existingRetryArguments.DelayBetweenRetriesMs;
            testMethod.CustomAttributes.Remove(originalAttribute);

            // Add the Retry attribute
            CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                originalAttribute.Name == FACT_ATTRIBUTE ||
                originalAttribute.Name == RETRY_FACT_ATTRIBUTE ||
                originalAttribute.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE
                    ? RETRY_FACT_ATTRIBUTE
                    : RETRY_THEORY_ATTRIBUTE);

            addRetryArguments(retryAttribute, maxRetries, delayBetweenRetriesMs);
            addSkipOnExceptionArgument(retryAttribute);

            // Copy arguments from the original attribute. If it's already a retry attribute, don't copy the retry arguments though
            for (int i = originalAttribute.Name == RETRY_FACT_ATTRIBUTE ||
                         originalAttribute.Name == RETRY_THEORY_ATTRIBUTE ||
                         originalAttribute.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE ||
                         originalAttribute.Name == RETRY_UNTAGGED_THEORY_ATTRIBUTE
                     ? existingRetryArguments.RetrySpecificArgumentCount
                     : 0;
                 i < originalAttribute.Arguments.Count;
                 i++)
            {
                retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
            }
        }

        private static void addSkipOnExceptionArgument(CodeAttributeDeclaration retryAttribute)
        {
            // Reqnroll.xUnit implements dynamic skipping by throwing Xunit.SkipException. Treat it as a skip rather
            // than a failed retry attempt without making the generator plugin load xUnit or xRetry at build time.
            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodeArrayCreateExpression(new CodeTypeReference(typeof(Type)),
                    new CodeExpression[]
                    {
                        new CodeTypeOfExpression(new CodeTypeReference("Xunit.SkipException"))
                    })));
        }

        private static void addRetryArguments(
            CodeAttributeDeclaration retryAttribute,
            int? maxRetries,
            int? delayBetweenRetriesMs)
        {
            if (!maxRetries.HasValue)
            {
                return;
            }

            retryAttribute.Arguments.Add(new CodeAttributeArgument(
                new CodePrimitiveExpression(maxRetries.Value)));

            if (delayBetweenRetriesMs.HasValue)
            {
                retryAttribute.Arguments.Add(new CodeAttributeArgument(
                    new CodePrimitiveExpression(delayBetweenRetriesMs.Value)));
            }
        }

        private static RetryAttributeArguments getExistingRetryArguments(CodeAttributeDeclaration attribute)
        {
            if (attribute.Name != RETRY_FACT_ATTRIBUTE &&
                attribute.Name != RETRY_THEORY_ATTRIBUTE &&
                attribute.Name != RETRY_UNTAGGED_FACT_ATTRIBUTE &&
                attribute.Name != RETRY_UNTAGGED_THEORY_ATTRIBUTE)
            {
                return RetryAttributeArguments.Empty;
            }

            int index = 0;
            int? maxRetries = tryGetIntArgument(attribute, index);
            if (maxRetries.HasValue)
            {
                index++;
            }

            int? delayBetweenRetriesMs = tryGetIntArgument(attribute, index);
            if (delayBetweenRetriesMs.HasValue)
            {
                index++;
            }

            if (index < attribute.Arguments.Count &&
                string.IsNullOrEmpty(attribute.Arguments[index].Name) &&
                attribute.Arguments[index].Value is CodeArrayCreateExpression)
            {
                index++;
            }

            return new RetryAttributeArguments(maxRetries, delayBetweenRetriesMs, index);
        }

        private static int? tryGetIntArgument(CodeAttributeDeclaration attribute, int index)
        {
            if (index >= attribute.Arguments.Count || !string.IsNullOrEmpty(attribute.Arguments[index].Name))
            {
                return null;
            }

            if (!(attribute.Arguments[index].Value is CodePrimitiveExpression primitiveExpression) ||
                !(primitiveExpression.Value is int value))
            {
                return null;
            }

            return value;
        }

        private static string stripLeadingAtSign(string s) => s.StartsWith("@") ? s.Substring(1) : s;

        private static bool isIgnoreTag(string tag) => tag.Equals(IGNORE_TAG, StringComparison.OrdinalIgnoreCase);

        private static bool isTestMethodAlreadyIgnored(CodeMemberMethod testMethod) =>
            testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .Where(a =>
                    a.Name == FACT_ATTRIBUTE ||
                    a.Name == THEORY_ATTRIBUTE ||
                    a.Name == RETRY_FACT_ATTRIBUTE ||
                    a.Name == RETRY_THEORY_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_THEORY_ATTRIBUTE)
                .Any(a => a.Arguments.OfType<CodeAttributeArgument>()
                    .Any(arg => string.Equals(arg.Name, "Skip", StringComparison.OrdinalIgnoreCase)));

        private void revertRetryAttribute(CodeMemberMethod testMethod)
        {
            CodeAttributeDeclaration retryAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a =>
                    a.Name == RETRY_FACT_ATTRIBUTE ||
                    a.Name == RETRY_THEORY_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE ||
                    a.Name == RETRY_UNTAGGED_THEORY_ATTRIBUTE);
            if (retryAttribute == null)
            {
                return;
            }

            RetryAttributeArguments existingRetryArguments = getExistingRetryArguments(retryAttribute);
            testMethod.CustomAttributes.Remove(retryAttribute);

            CodeAttributeDeclaration originalAttribute = CodeDomHelper.AddAttribute(testMethod,
                retryAttribute.Name == RETRY_FACT_ATTRIBUTE ||
                retryAttribute.Name == RETRY_UNTAGGED_FACT_ATTRIBUTE
                    ? FACT_ATTRIBUTE
                    : THEORY_ATTRIBUTE);

            // Copy over any non-retry-specific arguments (e.g. DisplayName)
            for (int i = existingRetryArguments.RetrySpecificArgumentCount; i < retryAttribute.Arguments.Count; i++)
            {
                originalAttribute.Arguments.Add(retryAttribute.Arguments[i]);
            }
        }

        private static string getRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t =>
                t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                // Is just "retry", or is "retry("... for params
                (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));

        private class RetryAttributeArguments
        {
            public static readonly RetryAttributeArguments Empty = new RetryAttributeArguments(null, null, 0);

            public RetryAttributeArguments(int? maxRetries, int? delayBetweenRetriesMs, int retrySpecificArgumentCount)
            {
                MaxRetries = maxRetries;
                DelayBetweenRetriesMs = delayBetweenRetriesMs;
                RetrySpecificArgumentCount = retrySpecificArgumentCount;
            }

            public int? MaxRetries { get; }
            public int? DelayBetweenRetriesMs { get; }
            public int RetrySpecificArgumentCount { get; }
        }
    }
}
