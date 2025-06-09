[//]: # (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)


[//]: \# (Src: xRetry.v3/header.md)

# xRetry.v3
Retry flickering test cases for xUnit v3.

[//]: \# (Src: ciBadge.md)

[![pipeline status](https://github.com/JoshKeegan/xRetry/actions/workflows/cicd.yaml/badge.svg)](https://github.com/JoshKeegan/xRetry/actions)


[//]: \# (Src: whenToUse.md)

## When to use this
This is intended for use on flickering tests, where the reason for failure is an external 
dependency and the failure is transient, e.g:
 - HTTP request over the network
 - Database call that could deadlock, timeout etc...

Whenever a test includes real-world infrastructure, particularly when communicated with via the
internet, there is a risk of the test randomly failing so we want to try and run it again. 
This is the intended use case of the library.  

If you have a test that covers some flaky code, where sporadic failures are caused by a bug, 
this library should **not** be used to cover it up!

[//]: \# (Src: xRetry.v3/usage.md)

## Usage: xUnit

Add the [`xRetry.v3` NuGet package](https://www.nuget.org/packages/xRetry.v3 "xRetry.v3 NuGet package") to your project.

### Facts

Above any `Fact` test case that should be retried, replace the `Fact` attribute, with
`RetryFact`, e.g:

```cs
using xRetry;

private static int defaultNumCalls = 0;

[RetryFact]
public void Default_Reaches3()
{
    defaultNumCalls++;

    Assert.Equal(3, defaultNumCalls);
}
```

This will attempt to run the test until it passes, up to 3 times by default.
You can optionally specify a number of times to attempt to run the test as an argument, e.g. `[RetryFact(5)]`.  

You can also optionally specify a delay between each retry (in milliseconds) as a second
parameter, e.g. `[RetryFact(5, 100)]` will run your test up to 5 times, waiting 100ms between each attempt.

### Theories

If you have used the library for retrying `Fact` tests, using it to retry a `Theory` should be very intuitive.  
Above any `Theory` test case that should be retried, replace the `Theory` attribute with `RetryTheory`, e.g:

```cs
// testId => numCalls
private static readonly Dictionary<int, int> defaultNumCalls = new Dictionary<int, int>()
{
    { 0, 0 },
    { 1, 0 }
};

[RetryTheory]
[InlineData(0)]
[InlineData(1)]
public void Default_Reaches3(int id)
{
    defaultNumCalls[id]++;

    Assert.Equal(3, defaultNumCalls[id]);
}
```

The same optional arguments (max attempts and delay between each retry) are supported as for facts, and can be used in the same way.

### Skipping tests at Runtime

[xUnit.v3 has added native support for skipping tests at runtime](https://xunit.net/docs/getting-started/v3/whats-new#dynamically-skippable-tests).
If you were using the xRetry skipping functionality with xUnit v2, you should find upgrading simple.

Most use cases will be covered by replacing `Skip.Always();` with `Assert.Skip("your reason");`.

If you are skipping custom exceptions, you will also need to change the way they are passed to the test attributes:  
`[RetryFact(typeof(TestException))]` would now be `[RetryFact(SkipExceptions = new[] {typeof(TestException)})]`  
`[RetryTheory(typeof(TestException))]` would now be `[RetryTheory(SkipExceptions = new[] {typeof(TestException)})]`

[//]: \# (Src: logs.md)

## Viewing retry logs
By default, you won't see whether your tests are being retried as we make this information available 
via the xunit diagnostic logs but test runners will hide these detailed logs by default.  
To enable them you must configure your xUnit test project to have `diagnosticMessages` set to `true` in the `xunit.runner.json`. 
See the [xUnit docs](https://xunit.net/docs/configuration-files) for a full setup guide of their config file, or see
this projects own unit tests which has been set up with this enabled.

[//]: \# (Src: issues.md)

## Issues
If you find a bug whilst using this library, please report it [on GitHub](https://github.com/JoshKeegan/xRetry/issues).