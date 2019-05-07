using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using XunitRetry.SpecFlowPlugin.XunitProviders;

namespace XunitRetry.SpecFlowPlugin
{
    public class TestGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private const string RetryTag = "retry";

        public TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper) { }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            string retryTag = GetRetryTag(scenarioCategories);
            if (retryTag != null)
            {
                int? maxRetries = GetMaxRetries(retryTag);

                // Remove the Fact attribute
                CodeAttributeDeclaration factAttribute = testMethod.CustomAttributes
                    .OfType<CodeAttributeDeclaration>().FirstOrDefault(a => a.Name == "Xunit.FactAttribute");
                if (factAttribute != null)
                {
                    testMethod.CustomAttributes.Remove(factAttribute);
                }

                // Add the Retry attribute
                CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                    "XunitRetry.RetryFact");

                if (maxRetries != null)
                {
                    retryAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(maxRetries)));
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
        }

        private string GetRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t => t.StartsWith(RetryTag, StringComparison.OrdinalIgnoreCase));

        private int? GetMaxRetries(string tag)
        {
            // Will look like retry(5)
            if (tag.Length <= RetryTag.Length + 2)
            {
                return null;
            }

            string strNum = tag.Substring(RetryTag.Length + 1, tag.Length - 2 - RetryTag.Length);
            return int.TryParse(strNum, out int num) ? (int?)num : null;
        }
    }
}
