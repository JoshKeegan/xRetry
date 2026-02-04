using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Reqnroll.BoDi;
using Reqnroll.Generator;
using Reqnroll.Generator.CodeDom;
using Reqnroll.Generator.UnitTestProvider;
using xRetry.v3.Reqnroll.Parsers;

namespace xRetry.v3.Reqnroll;

public class TestGeneratorProvider(
    CodeDomHelper codeDomHelper,
    IObjectContainer objectContainer,
    IRetryTagParser retryTagParser)
    : IUnitTestGeneratorProvider
{
    private const string RETRY_FACT_ATTRIBUTE = "xRetry.v3.RetryFact";
    private const string RETRY_THEORY_ATTRIBUTE = "xRetry.v3.RetryTheory";
    private const string FACT_ATTRIBUTE = "Xunit.FactAttribute";
    private const string THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";

    private const string SKIP_PROPERTY_NAME = "Skip";


    private readonly IUnitTestGeneratorProvider unitTestGeneratorProviderImplementation =
        objectContainer.Resolve<IUnitTestGeneratorProvider>("xunit3");

    public void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
    {
        unitTestGeneratorProviderImplementation.SetTestMethodIgnore(generationContext, testMethod);
    }

    // Called for scenario outlines, even when it has no tags.
    // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
    public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod,
        string scenarioTitle)
    {
        unitTestGeneratorProviderImplementation.SetRowTest(generationContext, testMethod, scenarioTitle);
        var featureTags = generationContext.Feature.Tags.Select(t => stripLeadingAtSign(t.Name)).ToArray();
        applyRetry(featureTags, testMethod);
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
        applyRetry(featureTags, testMethod);
    }

    // Called for both scenarios & scenario outlines, but only if it has tags
    public void SetTestMethodCategories(TestClassGenerationContext generationContext,
        CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
    {
        // Optimisation: Prevent multiple enumerations
        scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();
        unitTestGeneratorProviderImplementation.SetTestMethodCategories(generationContext, testMethod,
            scenarioCategories);
        applyRetry((string[]) scenarioCategories, testMethod);
    }

    /// <summary>
    ///     Apply retry tags to the current test
    /// </summary>
    /// <param name="tags">
    ///     Tags that haven't yet been processed. If the test has just been created these will be for the
    ///     feature, otherwise for the scenario
    /// </param>
    /// <param name="testMethod">Test method we are applying retries for</param>
    private void applyRetry(IList<string> tags, CodeMemberMethod testMethod)
    {
        // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
        if (isTestMethodAlreadyIgnored(testMethod)) return;

        var strRetryTag = getRetryTag(tags);
        if (strRetryTag == null) return;

        var retryTag = retryTagParser.Parse(strRetryTag);

        // Remove the original fact or theory attribute
        var originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
            .FirstOrDefault(a =>
                a.Name is FACT_ATTRIBUTE or THEORY_ATTRIBUTE or RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE);
        if (originalAttribute == null) return;

        testMethod.CustomAttributes.Remove(originalAttribute);

        // Add the Retry attribute
        var retryAttribute = codeDomHelper.AddAttribute(testMethod,
            originalAttribute.Name is FACT_ATTRIBUTE or RETRY_FACT_ATTRIBUTE
                ? RETRY_FACT_ATTRIBUTE
                : RETRY_THEORY_ATTRIBUTE);

        retryAttribute.Arguments.Add(new CodeAttributeArgument(
            new CodePrimitiveExpression(retryTag.MaxRetries ?? v3.RetryFactAttribute.DEFAULT_MAX_RETRIES)));
        retryAttribute.Arguments.Add(new CodeAttributeArgument(
            new CodePrimitiveExpression(retryTag.DelayBetweenRetriesMs ??
                                        v3.RetryFactAttribute.DEFAULT_DELAY_BETWEEN_RETRIES_MS)));

        // Copy arguments from the original attribute. If it's already a retry attribute, don't copy the retry arguments though
        for (var i = originalAttribute.Name is RETRY_FACT_ATTRIBUTE or RETRY_THEORY_ATTRIBUTE
                 ? retryAttribute.Arguments.Count
                 : 0;
             i < originalAttribute.Arguments.Count;
             i++)
            retryAttribute.Arguments.Add(originalAttribute.Arguments[i]);
    }

    private static string stripLeadingAtSign(string s)
    {
        return s.StartsWith("@") ? s.Substring(1) : s;
    }

    private static bool isTestMethodAlreadyIgnored(CodeMemberMethod testMethod)
    {
        var testAttributes = testMethod.CustomAttributes
            .OfType<CodeAttributeDeclaration>()
            .Where(attr => attr.Name is FACT_ATTRIBUTE or THEORY_ATTRIBUTE);

        return testAttributes.Select(attribute => attribute.Arguments.OfType<CodeAttributeArgument>()
                .Any(arg => string.Equals(arg.Name, SKIP_PROPERTY_NAME, StringComparison.InvariantCultureIgnoreCase)))
            .Any(containsSkip => containsSkip);
    }

    private static string? getRetryTag(IEnumerable<string> tags)
    {
        return tags.FirstOrDefault(t =>
            t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
            // Is just "retry", or is "retry("... for params
            (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));
    }
}