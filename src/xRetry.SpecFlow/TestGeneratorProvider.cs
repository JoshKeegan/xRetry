using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using xRetry.SpecFlow.XunitProviders;

namespace xRetry.SpecFlow
{
    public class TestGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private const string RETRY_TAG = "retry";

        public TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper) { }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            string retryTag = getRetryTag(scenarioCategories);
            if (retryTag != null)
            {
                int? maxRetries = getMaxRetries(retryTag);

                // Remove the Fact attribute
                CodeAttributeDeclaration factAttribute = testMethod.CustomAttributes
                    .OfType<CodeAttributeDeclaration>().FirstOrDefault(a => a.Name == "Xunit.FactAttribute");
                if (factAttribute != null)
                {
                    testMethod.CustomAttributes.Remove(factAttribute);
                }

                // Add the Retry attribute
                CodeAttributeDeclaration retryAttribute = CodeDomHelper.AddAttribute(testMethod,
                    "xRetry.RetryFact");

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

        private string getRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t => t.StartsWith(RETRY_TAG, StringComparison.OrdinalIgnoreCase));

        private int? getMaxRetries(string tag)
        {
            // Will look like retry(5)
            if (tag.Length <= RETRY_TAG.Length + 2)
            {
                return null;
            }

            string strNum = tag.Substring(RETRY_TAG.Length + 1, tag.Length - 2 - RETRY_TAG.Length);
            return int.TryParse(strNum, out int num) ? (int?)num : null;
        }
    }
}
