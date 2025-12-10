using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Xunit.v3;
using xRetry.v3;
using Xunit.Sdk;
using System.Reflection;

namespace UnitTests.v3;

public class RetryTestCaseRunnerTests
{
    [Fact]
    public void ToTestCaseStarting_WithFullyPopulatedTestCase_MapsAllProperties()
    {
        var testCase = CreateRetryTestCase(
            maxRetries: 5,
            delayBetweenRetriesMs: 100,
            @explicit: true,
            skipReason: "Test skip reason",
            sourceFilePath: @"C:\Source\TestFile.cs",
            sourceLineNumber: 42,
            traits: new Dictionary<string, HashSet<string>>
            {
                { "Category", new HashSet<string> { "Unit", "Fast" } },
                { "Priority", new HashSet<string> { "High" } }
            });

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Should().NotBeNull();
        result.AssemblyUniqueID.Should().NotBeNullOrEmpty();
        result.TestCollectionUniqueID.Should().NotBeNullOrEmpty();
        result.TestCaseUniqueID.Should().NotBeNullOrEmpty();
        result.TestClassUniqueID.Should().NotBeNullOrEmpty();
        result.TestMethodUniqueID.Should().NotBeNullOrEmpty();
        result.Explicit.Should().BeTrue();
        result.SkipReason.Should().Be("Test skip reason");
        result.SourceFilePath.Should().Be(@"C:\Source\TestFile.cs");
        result.SourceLineNumber.Should().Be(42);
        result.TestCaseDisplayName.Should().NotBeNullOrEmpty();
        result.TestClassName.Should().NotBeNullOrEmpty();
        result.TestClassNamespace.Should().NotBeNullOrEmpty();
        result.TestClassSimpleName.Should().NotBeNullOrEmpty();
        result.TestMethodName.Should().NotBeNullOrEmpty();
        result.Traits.Should().ContainKey("Category");
        result.Traits.Should().ContainKey("Priority");
    }

    [Fact]
    public void ToTestCaseStarting_WithExplicitFalse_SetsExplicitToFalse()
    {
        var testCase = CreateRetryTestCase(@explicit: false);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Explicit.Should().BeFalse();
    }

    [Fact]
    public void ToTestCaseStarting_WithNullSkipReason_SetsSkipReasonToNull()
    {
        var testCase = CreateRetryTestCase(skipReason: null);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.SkipReason.Should().BeNull();
    }

    [Fact]
    public void ToTestCaseStarting_WithEmptyTraits_PreservesEmptyTraits()
    {
        var emptyTraits = new Dictionary<string, HashSet<string>>();
        var testCase = CreateRetryTestCase(traits: emptyTraits);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Traits.Should().NotBeNull();
        result.Traits.Should().BeEmpty();
    }

    [Fact]
    public void ToTestCaseStarting_WithNullSourceFilePath_SetsSourceFilePathToNull()
    {
        var testCase = CreateRetryTestCase(sourceFilePath: null);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.SourceFilePath.Should().BeNull();
    }

    [Fact]
    public void ToTestCaseStarting_WithZeroSourceLineNumber_SetsSourceLineNumberToZero()
    {
        var testCase = CreateRetryTestCase(sourceLineNumber: 0);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.SourceLineNumber.Should().Be(0);
    }

    [Fact]
    public void ToTestCaseStarting_WithNullSourceLineNumber_PreservesNullValue()
    {
        var testCase = CreateRetryTestCase(sourceLineNumber: null);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.SourceLineNumber.Should().BeNull();
    }

    [Fact]
    public void ToTestCaseStarting_WithMinimalTestCase_CreatesValidTestCaseStarting()
    {
        var testCase = CreateRetryTestCase();

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ITestCaseStarting>();
    }

    [Fact]
    public void ToTestCaseStarting_MapsAllUniqueIDs()
    {
        var testCase = CreateRetryTestCase();

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.AssemblyUniqueID.Should().Be(testCase.TestCollection.TestAssembly.UniqueID);
        result.TestCollectionUniqueID.Should().Be(testCase.TestCollection.UniqueID);
        result.TestCaseUniqueID.Should().Be(testCase.UniqueID);
        result.TestClassUniqueID.Should().Be(testCase.TestClass?.UniqueID);
        result.TestMethodUniqueID.Should().Be(testCase.TestMethod?.UniqueID);
    }

    [Fact]
    public void ToTestCaseStarting_MapsTestCaseDisplayName()
    {
        var testCase = CreateRetryTestCase();

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.TestCaseDisplayName.Should().Be(testCase.TestCaseDisplayName);
    }

    [Fact]
    public void ToTestCaseStarting_MapsTestClassMetadata()
    {
        var testCase = CreateRetryTestCase();

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.TestClassName.Should().Be(testCase.TestClassName);
        result.TestClassNamespace.Should().Be(testCase.TestClassNamespace);
        result.TestClassSimpleName.Should().Be(testCase.TestClassSimpleName);
        result.TestClassMetadataToken.Should().Be(testCase.TestClassMetadataToken);
    }

    [Fact]
    public void ToTestCaseStarting_MapsTestMethodMetadata()
    {
        var testCase = CreateRetryTestCase();

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.TestMethodName.Should().Be(testCase.TestMethod?.MethodName);
        result.TestMethodArity.Should().Be(testCase.TestMethodArity);
        result.TestMethodMetadataToken.Should().Be(testCase.TestMethodMetadataToken);
        result.TestMethodReturnTypeVSTest.Should().Be(testCase.TestMethodReturnTypeVSTest);
    }

    [Fact]
    public void ToTestCaseStarting_WithMultipleTraits_PreservesAllTraits()
    {
        var traits = new Dictionary<string, HashSet<string>>
        {
            { "Category", new HashSet<string> { "Unit", "Integration", "Fast" } },
            { "Priority", new HashSet<string> { "High" } },
            { "Owner", new HashSet<string> { "TeamA", "TeamB" } }
        };
        var testCase = CreateRetryTestCase(traits: traits);

        var result = RetryTestCaseRunner.ToTestCaseStarting(testCase);

        result.Traits.Should().HaveCount(3);
        result.Traits["Category"].Should().HaveCount(3);
        result.Traits["Owner"].Should().HaveCount(2);
    }

    #region ToTestCaseFinished Tests

    [Fact]
    public void ToTestCaseFinished_WithSuccessfulTest_MapsAllProperties()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 1,
            Failed = 0,
            Skipped = 0,
            NotRun = 0,
            Time = 0.5m
        };
        var start = DateTimeOffset.UtcNow.AddSeconds(-1);

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.Should().NotBeNull();
        result.AssemblyUniqueID.Should().Be(testCase.TestCollection.TestAssembly.UniqueID);
        result.TestCollectionUniqueID.Should().Be(testCase.TestCollection.UniqueID);
        result.TestCaseUniqueID.Should().Be(testCase.UniqueID);
        result.TestClassUniqueID.Should().Be(testCase.TestClass?.UniqueID);
        result.TestMethodUniqueID.Should().Be(testCase.TestMethod?.UniqueID);
        result.TestsFailed.Should().Be(0);
        result.TestsSkipped.Should().Be(0);
        result.TestsNotRun.Should().Be(0);
        result.TestsTotal.Should().Be(1);
    }

    [Fact]
    public void ToTestCaseFinished_WithFailedTest_MapsFailureCount()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 1,
            Failed = 1,
            Skipped = 0,
            NotRun = 0
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsFailed.Should().Be(1);
        result.TestsTotal.Should().Be(1);
    }

    [Fact]
    public void ToTestCaseFinished_WithSkippedTest_MapsSkippedCount()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 1,
            Failed = 0,
            Skipped = 1,
            NotRun = 0
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsSkipped.Should().Be(1);
        result.TestsTotal.Should().Be(1);
    }

    [Fact]
    public void ToTestCaseFinished_WithNotRunTest_MapsNotRunCount()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 1,
            Failed = 0,
            Skipped = 0,
            NotRun = 1
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsNotRun.Should().Be(1);
        result.TestsTotal.Should().Be(1);
    }

    [Fact]
    public void ToTestCaseFinished_CalculatesExecutionTime()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary { Total = 1 };
        var start = DateTimeOffset.UtcNow.AddSeconds(-2);

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.ExecutionTime.Should().BeGreaterThan(1.9m);
        result.ExecutionTime.Should().BeLessThan(3.0m);
    }

    [Fact]
    public void ToTestCaseFinished_WithZeroElapsedTime_ReturnsSmallExecutionTime()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary { Total = 1 };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.ExecutionTime.Should().BeGreaterOrEqualTo(0);
        result.ExecutionTime.Should().BeLessThan(1.0m);
    }

    [Fact]
    public void ToTestCaseFinished_MapsAllUniqueIDs()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary { Total = 1 };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.AssemblyUniqueID.Should().Be(testCase.TestCollection.TestAssembly.UniqueID);
        result.TestCollectionUniqueID.Should().Be(testCase.TestCollection.UniqueID);
        result.TestCaseUniqueID.Should().Be(testCase.UniqueID);
        result.TestClassUniqueID.Should().Be(testCase.TestClass?.UniqueID);
        result.TestMethodUniqueID.Should().Be(testCase.TestMethod?.UniqueID);
    }

    [Fact]
    public void ToTestCaseFinished_WithMultipleTests_MapsTotalCount()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 5,
            Failed = 1,
            Skipped = 1,
            NotRun = 0
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsTotal.Should().Be(5);
        result.TestsFailed.Should().Be(1);
        result.TestsSkipped.Should().Be(1);
        result.TestsNotRun.Should().Be(0);
    }

    [Fact]
    public void ToTestCaseFinished_WithAllTestsFailed_MapsCorrectly()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 3,
            Failed = 3,
            Skipped = 0,
            NotRun = 0
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsTotal.Should().Be(3);
        result.TestsFailed.Should().Be(3);
        result.TestsSkipped.Should().Be(0);
        result.TestsNotRun.Should().Be(0);
    }

    [Fact]
    public void ToTestCaseFinished_WithMixedResults_MapsAllCounts()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary
        {
            Total = 10,
            Failed = 2,
            Skipped = 3,
            NotRun = 1
        };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.TestsTotal.Should().Be(10);
        result.TestsFailed.Should().Be(2);
        result.TestsSkipped.Should().Be(3);
        result.TestsNotRun.Should().Be(1);
    }

    [Fact]
    public void ToTestCaseFinished_WithLongRunningTest_CalculatesCorrectExecutionTime()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary { Total = 1 };
        var start = DateTimeOffset.UtcNow.AddSeconds(-10);

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.ExecutionTime.Should().BeGreaterThan(9.5m);
        result.ExecutionTime.Should().BeLessThan(11.0m);
    }

    [Fact]
    public void ToTestCaseFinished_ReturnsIMessageSinkMessage()
    {
        var testCase = CreateRetryTestCase();
        var summary = new RunSummary { Total = 1 };
        var start = DateTimeOffset.UtcNow;

        var result = RetryTestCaseRunner.ToTestCaseFinished(testCase, summary, start);

        result.Should().BeAssignableTo<IMessageSinkMessage>();
    }

    #endregion

    public static RetryTestCase CreateRetryTestCase(
        int maxRetries = 3,
        int delayBetweenRetriesMs = 0,
        bool @explicit = false,
        string skipReason = null,
        string sourceFilePath = null,
        int? sourceLineNumber = null,
        Dictionary<string, HashSet<string>> traits = null)
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
