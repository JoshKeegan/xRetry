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
        private readonly IRetryTagParser retryTagParser;
        private readonly ILogger logger;
        private readonly IRetrySettings retrySettings;

        public TestGeneratorProvider(
            CodeDomHelper codeDomHelper,
            ProjectSettings projectSettings,
            IRetryTagParser retryTagParser,
            ILogger logger,
            IRetrySettings retrySettings)
            : base(codeDomHelper, projectSettings)
        {
            this.retryTagParser = retryTagParser;
            this.logger = logger;
            this.retrySettings = retrySettings;
        }

        public override void SetTestMethodCategories(
            TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod,
            IEnumerable<string> scenarioCategories)
        {
            logger.Log($"{nameof(SetTestMethodCategories)}");

            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);
            
            // Remove all retry attribute to have a clean desk 
            var removeThisAttribute = testMethod.CustomAttributes
                .OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(declaration => declaration.Name.StartsWith(Constants.RETRY_ROOT_ATTRIBUTE));
                
            if(removeThisAttribute != null)
            {
                testMethod.CustomAttributes.Remove(removeThisAttribute);
            }

            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            //  This allows for the new (for SpecFlow 3.1.x) implementation that relies on Xunit.SkippableFact to still work, as it
            //  too will replace the attribute for running the test with a custom one.
            if (IsIgnored(generationContext, scenarioCategories))
            {
                logger.Log("test must be skipped");
                return;
            }

            var strRetryTag = GetRetryTag(scenarioCategories);
            if (strRetryTag == null && !retrySettings.Global)
            {
                return;
            }

            logger.Log($"strRetryTag : {strRetryTag}");

            if (retrySettings.isVerbose)
            {
                var attributes = testMethod.CustomAttributes
                    .OfType<CodeAttributeDeclaration>()
                    .Select(f => f.Name)
                    .Aggregate((m, n) => $"{m}, {n}");
                logger.Log($"attributes : {attributes}");
            }

            var globalRetryAttribute = testMethod.CustomAttributes
                .OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(declaration => declaration.Name.StartsWith(Constants.RETRY_ATTRIBUTE));

            if (globalRetryAttribute != null)
            {
                testMethod.CustomAttributes.Remove(globalRetryAttribute);
            }

            var retryTag = retryTagParser.Parse(strRetryTag);

            // Remove the original fact or theory attribute
            var originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == FACT_ATTRIBUTE || a.Name == THEORY_ATTRIBUTE);
            if (originalAttribute == null)
            {
                return;
            }

            testMethod.CustomAttributes.Remove(originalAttribute);

            // Add the Retry attribute
            var retryAttribute = CodeDomHelper.AddAttribute(
                testMethod,
                "xRetry.Retry" + (originalAttribute.Name == FACT_ATTRIBUTE ? "Fact" : "Theory"));

            if (retryTag.MaxRetries != null)
            {
                retryAttribute.Arguments.Add(
                    new CodeAttributeArgument(new CodePrimitiveExpression(retryTag.MaxRetries)));

                if (retryTag.DelayBetweenRetriesMs != null)
                    retryAttribute.Arguments.Add(
                        new CodeAttributeArgument(new CodePrimitiveExpression(retryTag.DelayBetweenRetriesMs)));
            }

            // Copy arguments from the original attribute
            for (var i = 0; i < originalAttribute.Arguments.Count; i++)
            {
                retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
                logger.Log($"Attribute added : {originalAttribute.Arguments[i].Name}");
            }
        }

        private static bool IsIgnored(TestClassGenerationContext generationContext, IEnumerable<string> tags)
        {
            return generationContext.Feature.Tags
                    .Select(t => StripLeadingAtSign(t.Name))
                    .Any(IsIgnoreTag) ||
                tags.Any(IsIgnoreTag);
        }

        private static string StripLeadingAtSign(string s)
        {
            return s.StartsWith("@") ? s.Substring(1) : s;
        }

        private static bool IsIgnoreTag(string tag)
        {
            return tag.Equals(Constants.IGNORE_TAG, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetRetryTag(IEnumerable<string> tags)
        {
            return tags.FirstOrDefault(
                t => t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                    // Is just "retry", or is "retry("... for params
                    (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
        }
    }
}