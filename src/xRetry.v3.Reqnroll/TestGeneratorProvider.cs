using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Reqnroll.BoDi;
using Reqnroll.Generator;
using Reqnroll.Generator.CodeDom;
using Reqnroll.Generator.Interfaces;
using Reqnroll.Generator.UnitTestProvider;
using xRetry.v3.Reqnroll.Parsers;

namespace xRetry.v3.Reqnroll;

public class TestGeneratorProvider(
    CodeDomHelper codeDomHelper,
    IObjectContainer objectContainer,
    ProjectSettings projectSettings,
    IRetryTagParser retryTagParser)
    : IUnitTestGeneratorProvider
{
    private const string RETRY_FACT_ATTRIBUTE = "xRetry.v3.RetryFact";
    private const string RETRY_THEORY_ATTRIBUTE = "xRetry.v3.RetryTheory";
    private const string FACT_ATTRIBUTE = "Xunit.FactAttribute";
    private const string THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";

    private const string SKIP_PROPERTY_NAME = "Skip";


    private readonly RetryDefaults retryDefaults = RetryDefaults.Load(projectSettings.ProjectFolder);
    private readonly IUnitTestGeneratorProvider unitTestGeneratorProviderImplementation =
        objectContainer.Resolve<IUnitTestGeneratorProvider>("xunit3");

    // @ignore is handled after the first retry pass, so retryUntaggedScenarios may have
    // already changed Fact/Theory to RetryFact/RetryTheory. Restore the xUnit attribute
    // before delegating so the underlying provider can set Skip; the later retry pass
    // sees Skip and leaves the ignored test un-retried.
    public void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
    {
        revertRetryAttribute(testMethod);
        unitTestGeneratorProviderImplementation.SetTestMethodIgnore(generationContext, testMethod);
    }

    // Called for scenario outlines, even when it has no tags.
    // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
    public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
        string scenarioTitle)
    {
        unitTestGeneratorProviderImplementation.SetRowTest(generationContext, testMethod, scenarioTitle);
        var featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();
        applyRetry(featureTags, Enumerable.Empty<string>(), testMethod, applyGlobalRetryDefaults: true);
    }

    public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
        IEnumerable<string> arguments,
        IEnumerable<string> tags, bool isIgnored)
    {
        unitTestGeneratorProviderImplementation.SetRow(generationContext, testMethod, arguments, tags, isIgnored);
    }

    public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
        string scenarioTitle,
        string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
    {
        unitTestGeneratorProviderImplementation.SetTestMethodAsRow(generationContext, testMethod, scenarioTitle,
            exampleSetName, variantName, arguments);
    }

    public void MarkCodeMethodInvokeExpressionAsAwait(CodeMethodInvokeExpression expression)
    {
        unitTestGeneratorProviderImplementation.MarkCodeMethodInvokeExpressionAsAwait(expression);
    }

    // Called for scenarios, even when it has no tags.
    // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
    public UnitTestGeneratorTraits GetTraits()
    {
        return unitTestGeneratorProviderImplementation.GetTraits();
    }

    public void SetTestClass(TestClassGenerationContext generationContext, string featureTitle,
        string featureDescription)
    {
        unitTestGeneratorProviderImplementation.SetTestClass(generationContext, featureTitle, featureDescription);
    }

    public void SetTestClassCategories(TestClassGenerationContext generationContext,
        IEnumerable<string> featureCategories)
    {
        unitTestGeneratorProviderImplementation.SetTestClassCategories(generationContext, featureCategories);
    }

    public void SetTestClassIgnore(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestClassIgnore(generationContext);
    }

    public void FinalizeTestClass(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.FinalizeTestClass(generationContext);
    }

    public void SetTestClassNonParallelizable(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestClassNonParallelizable(generationContext);
    }

    public void SetTestMethodNonParallelizable(TestClassGenerationContext generationContext,
        CodeMemberMethod testMethod)
    {
        unitTestGeneratorProviderImplementation.SetTestMethodNonParallelizable(generationContext, testMethod);
    }

    public void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestClassInitializeMethod(generationContext);
    }

    public void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestClassCleanupMethod(generationContext);
    }

    public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestInitializeMethod(generationContext);
    }

    public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
    {
        unitTestGeneratorProviderImplementation.SetTestCleanupMethod(generationContext);
    }

    public void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
        string friendlyTestName)
    {
        unitTestGeneratorProviderImplementation.SetTestMethod(generationContext, testMethod, friendlyTestName);
        var featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();
        applyRetry(featureTags, Enumerable.Empty<string>(), testMethod, applyGlobalRetryDefaults: true);
    }

    // Called for both scenarios & scenario outlines, but only if it has tags
    public void SetTestMethodCategories(TestClassGenerationContext generationContext,
        CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
    {
        // Optimisation: Prevent multiple enumerations
        scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();
        unitTestGeneratorProviderImplementation.SetTestMethodCategories(generationContext, testMethod,
            scenarioCategories);
        IEnumerable<string> featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name));
        applyRetry((string[]) scenarioCategories, featureTags, testMethod, applyGlobalRetryDefaults: false);
    }

    /// <summary>
    ///     Apply retry tags to the current test
    /// </summary>
    /// <param name="tags">
    ///     Tags that haven't yet been processed. If the test has just been created these will be for the
    ///     feature, otherwise for the scenario
    /// </param>
    /// <param name="processedTags">
    ///     Tags that have already been processed. This is only used so that a feature-level
    ///     <c>@ignore</c> continues to prevent retries when scenario tags are processed later.
    /// </param>
    /// <param name="testMethod">Test method we are applying retries for</param>
    /// <param name="applyGlobalRetryDefaults">
    ///     Whether the global retry defaults should be applied to untagged scenarios
    /// </param>
    private void applyRetry(
        IList<string> tags,
        IEnumerable<string> processedTags,
        CodeMemberMethod testMethod,
        bool applyGlobalRetryDefaults)
    {
        // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
        if (isTestMethodAlreadyIgnored(testMethod) || tags.Any(isIgnoreTag) || processedTags.Any(isIgnoreTag)) return;

        if (applyGlobalRetryDefaults && retryDefaults.RetryUntaggedScenarios)
        {
            replaceWithRetryAttribute(testMethod, null, null);
        }

        var strRetryTag = getRetryTag(tags);
        if (strRetryTag == null) return;

        var retryTag = retryTagParser.Parse(strRetryTag);
        replaceWithRetryAttribute(testMethod, retryTag.MaxRetries, retryTag.DelayBetweenRetriesMs);
    }

    private void replaceWithRetryAttribute(
        CodeMemberMethod testMethod,
        int? maxRetries,
        int? delayBetweenRetriesMs)
    {
        if (isTestMethodAlreadyIgnored(testMethod)) return;

        // Remove the original fact or theory attribute
        var originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
            .FirstOrDefault(a =>
                a.Name is FACT_ATTRIBUTE or THEORY_ATTRIBUTE or RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE);
        if (originalAttribute == null) return;

        var existingRetryArguments = getExistingRetryArguments(originalAttribute);
        maxRetries ??= existingRetryArguments.MaxRetries;
        delayBetweenRetriesMs ??= existingRetryArguments.DelayBetweenRetriesMs;
        testMethod.CustomAttributes.Remove(originalAttribute);

        // Add the Retry attribute
        var retryAttribute = codeDomHelper.AddAttribute(testMethod,
            originalAttribute.Name is FACT_ATTRIBUTE or RETRY_FACT_ATTRIBUTE
                ? RETRY_FACT_ATTRIBUTE
                : RETRY_THEORY_ATTRIBUTE);

        addRetryArguments(retryAttribute, maxRetries, delayBetweenRetriesMs);

        // Copy arguments from the original attribute. If it's already a retry attribute, don't copy the retry arguments though
        for (var i = originalAttribute.Name is RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE
                 ? existingRetryArguments.RetrySpecificArgumentCount
                 : 0;
             i < originalAttribute.Arguments.Count;
             i++)
            retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
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
        if (attribute.Name is not (RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE))
        {
            return RetryAttributeArguments.Empty;
        }

        var index = 0;
        var maxRetries = tryGetIntArgument(attribute, index);
        if (maxRetries.HasValue)
        {
            index++;
        }

        var delayBetweenRetriesMs = tryGetIntArgument(attribute, index);
        if (delayBetweenRetriesMs.HasValue)
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

        return attribute.Arguments[index].Value is CodePrimitiveExpression { Value: int value }
            ? value
            : null;
    }

    private static string stripLeadingAtSign(string s)
    {
        return s.StartsWith("@") ? s.Substring(1) : s;
    }

    private static bool isIgnoreTag(string tag)
    {
        return tag.Equals("ignore", StringComparison.OrdinalIgnoreCase);
    }

    private static bool isTestMethodAlreadyIgnored(CodeMemberMethod testMethod)
    {
        var testAttributes = testMethod.CustomAttributes
            .OfType<CodeAttributeDeclaration>()
            .Where(attr => attr.Name is FACT_ATTRIBUTE or THEORY_ATTRIBUTE or RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE);

        return testAttributes.Select(attribute => attribute.Arguments.OfType<CodeAttributeArgument>()
                .Any(arg => string.Equals(arg.Name, SKIP_PROPERTY_NAME, StringComparison.InvariantCultureIgnoreCase)))
            .Any(containsSkip => containsSkip);
    }

    private void revertRetryAttribute(CodeMemberMethod testMethod)
    {
        var retryAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
            .FirstOrDefault(a => a.Name is RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE);
        if (retryAttribute == null)
        {
            return;
        }

        var existingRetryArguments = getExistingRetryArguments(retryAttribute);
        testMethod.CustomAttributes.Remove(retryAttribute);

        var originalAttribute = codeDomHelper.AddAttribute(testMethod,
            retryAttribute.Name == RETRY_FACT_ATTRIBUTE ? FACT_ATTRIBUTE : THEORY_ATTRIBUTE);

        // Copy over any non-retry-specific arguments (e.g. DisplayName)
        for (var i = existingRetryArguments.RetrySpecificArgumentCount; i < retryAttribute.Arguments.Count; i++)
        {
            originalAttribute.Arguments.Add(retryAttribute.Arguments[i]);
        }
    }

    private static string? getRetryTag(IEnumerable<string> tags)
    {
        return tags.FirstOrDefault(t =>
            t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
            // Is just "retry", or is "retry("... for params
            (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
    }

    private sealed class RetryAttributeArguments(int? maxRetries, int? delayBetweenRetriesMs, int retrySpecificArgumentCount)
    {
        public static readonly RetryAttributeArguments Empty = new(null, null, 0);

        public int? DelayBetweenRetriesMs { get; } = delayBetweenRetriesMs;
        public int? MaxRetries { get; } = maxRetries;
        public int RetrySpecificArgumentCount { get; } = retrySpecificArgumentCount;
    }
}
