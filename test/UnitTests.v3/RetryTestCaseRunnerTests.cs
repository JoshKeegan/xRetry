using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using xRetry.v3;
using Xunit;
using Xunit.v3;

namespace UnitTests.v3;

public class RetryTestCaseRunnerTests
{
    [Fact]
    public void ToTestCaseStarting_MapsAllProperties()
    {
        var traits = new Dictionary<string, HashSet<string>>
        {
            { "Category", new HashSet<string> { "Unit", "Fast" } },
            { "Priority", new HashSet<string> { "High" } }
        };

        var testCase = CreateRetryTestCase(
            maxRetries: 5,
            delayBetweenRetriesMs: 100,
            @explicit: true,
            skipReason: "Test skip reason",
            sourceFilePath: @"C:\Source\TestFile.cs",
            sourceLineNumber: 42,
            traits: traits);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Should().NotBeNull();
        result.AssemblyUniqueID.Should().Be(testCase.TestCollection.TestAssembly.UniqueID);
        result.TestCollectionUniqueID.Should().Be(testCase.TestCollection.UniqueID);
        result.TestCaseUniqueID.Should().Be(testCase.UniqueID);
        result.TestClassUniqueID.Should().Be(testCase.TestClass.UniqueID);
        result.TestMethodUniqueID.Should().Be(testCase.TestMethod.UniqueID);
        result.Explicit.Should().BeTrue();
        result.SkipReason.Should().Be(testCase.SkipReason);
        result.SourceFilePath.Should().Be(testCase.SourceFilePath);
        result.SourceLineNumber.Should().Be(testCase.SourceLineNumber);
        result.TestCaseDisplayName.Should().Be(testCase.TestCaseDisplayName);
        result.TestClassName.Should().Be(testCase.TestClassName);
        result.TestClassNamespace.Should().Be(testCase.TestClassNamespace);
        result.TestClassSimpleName.Should().Be(testCase.TestClassSimpleName);
        result.TestMethodName.Should().Be(testCase.TestMethodName);

        var resultTraits = result.Traits.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToHashSet());
        resultTraits.Should().BeEquivalentTo(traits);
    }

    [Fact]
    public void ToTestCaseFinished_MapsAllProperties()
    {
        var stopwatch = Stopwatch.StartNew();

        var testCase = CreateRetryTestCase(
            maxRetries: 5,
            delayBetweenRetriesMs: 100,
            @explicit: true,
            skipReason: "Test skip reason",
            sourceFilePath: @"C:\Source\TestFile.cs",
            sourceLineNumber: 42,
            traits: []);

        var summary = new RunSummary
        {
            Total = 1,
            Failed = 0,
            Skipped = 0,
            NotRun = 0,
            Time = 0.5m
        };

        var before = (decimal) stopwatch.Elapsed.TotalSeconds;
        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, stopwatch);

        result.Should().NotBeNull();
        result.AssemblyUniqueID.Should().Be(testCase.TestCollection.TestAssembly.UniqueID);
        result.TestCollectionUniqueID.Should().Be(testCase.TestCollection.UniqueID);
        result.TestCaseUniqueID.Should().Be(testCase.UniqueID);
        result.TestClassUniqueID.Should().Be(testCase.TestClass.UniqueID);
        result.TestMethodUniqueID.Should().Be(testCase.TestMethod.UniqueID);
        result.TestsFailed.Should().Be(0);
        result.TestsSkipped.Should().Be(0);
        result.TestsNotRun.Should().Be(0);
        result.TestsTotal.Should().Be(1);
        result.ExecutionTime.Should().BeGreaterThanOrEqualTo(before);
    }

    public static RetryTestCase CreateRetryTestCase(
        int maxRetries,
        int delayBetweenRetriesMs,
        bool @explicit,
        string skipReason,
        string sourceFilePath,
        int? sourceLineNumber,
        Dictionary<string, HashSet<string>> traits)
    {
        var testAssembly = new XunitTestAssembly(Assembly.GetExecutingAssembly());
        var testCollection = new XunitTestCollection(
            testAssembly,
            null,
            false,
            "Test Collection");

        var testClass = new XunitTestClass(
            typeof(RetryTestCaseRunnerTests),
            testCollection
            );

        var methodInfo = typeof(RetryTestCaseRunnerTests).GetMethod(
            nameof(CreateRetryTestCase),
            BindingFlags.Static | BindingFlags.Public);
        var testMethod = new XunitTestMethod(
            testClass,
            methodInfo,
            []
            );

        return new RetryTestCase(
            maxRetries: maxRetries,
            delayBetweenRetriesMs: delayBetweenRetriesMs,
            testMethod: testMethod,
            testCaseDisplayName: $"Test Display Name",
            uniqueId: Guid.NewGuid().ToString(),
            @explicit: @explicit,
            skipExceptions: [],
            skipReason: skipReason,
            skipType: null,
            skipUnless: null,
            skipWhen: null,
            traits: traits,
            testMethodArguments: null,
            sourceFilePath: sourceFilePath,
            sourceLineNumber: sourceLineNumber,
            timeout: null);
    }
}
