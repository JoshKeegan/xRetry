using System.CodeDom;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace xRetry.SpecFlow
{
    public class RetryAttributeGenerator : ITestMethodDecorator
    {
        private readonly ILogger logger;
        private readonly IRetrySettings retrySettings;

        public RetryAttributeGenerator(
            IRetrySettings retrySettings,
            ILogger logger)
        {
            this.retrySettings = retrySettings;
            this.logger = logger;
        }

        public int Priority => PriorityValues.Normal;

        public bool CanDecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            logger.Log("===============================================");
            logger.Log($"{nameof(CanDecorateFrom)} : {testMethod.Name}");

            if (!retrySettings.Global)
            {
                logger.Log("Global is disable");
                return false;
            }

            if (retrySettings.isVerbose)
            {
                var customAttributes = testMethod.CustomAttributes
                    .OfType<CodeAttributeDeclaration>()
                    .Select(declaration => declaration.Name)
                    .DefaultIfEmpty("")
                    .Aggregate((m, n) => $"{m}, {n}");
                logger.Log($"CustomAttributes : {customAttributes}");
            }

            var isTagMustBeAdded = !generationContext.Feature.Tags
                .Any(tag => Regex.IsMatch(tag.Name, "@ignore", RegexOptions.Compiled | RegexOptions.IgnoreCase));

            isTagMustBeAdded = isTagMustBeAdded &&
                !testMethod.CustomAttributes
                    .OfType<CodeAttributeDeclaration>()
                    .Any(declaration => declaration.Name.StartsWith(Constants.RETRY_ROOT_ATTRIBUTE));

            logger.Log($"isTagMustBeAdded : {isTagMustBeAdded}");

            return isTagMustBeAdded;
        }

        public void DecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            logger.Log($"{nameof(DecorateFrom)} : {testMethod.Name}");

            var tagToAdd = string.Empty;

            for (var i = testMethod.CustomAttributes.Count - 1; i >= 0; i--)
            {
                var testMethodCustomAttribute = testMethod.CustomAttributes[i];
                if (Regex.IsMatch(
                    testMethodCustomAttribute.AttributeType.BaseType,
                    "(Fact|Theory)(Attribute)?$",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    tagToAdd = testMethodCustomAttribute.Name.Contains("Theory")
                        ? Constants.RETRY_THEORY_ATTRIBUTE
                        : Constants.RETRY_ATTRIBUTE;

                    testMethod.CustomAttributes.Remove(testMethodCustomAttribute);
                }
            }

            testMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    tagToAdd,
                    new CodeAttributeArgument(new CodePrimitiveExpression(retrySettings.MaxRetry)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(retrySettings.DelayBetweenRetriesMs))
                )
            );
        }
    }
}