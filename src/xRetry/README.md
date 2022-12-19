[//]: # (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)


[//]: \# (Src: xRetry/header.md)

# xRetry
Retry flickering test cases for xUnit.

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

[//]: \# (Src: xRetry/usage.md)

## Usage: xUnit
Add the `xRetry` nuget package to your project.

### Facts
Above any `Fact` test case that should be retried, replace the `Fact` attribute, with 
`RetryFact`, e.g:
```cs
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
In addition to retries, `RetryFact` and `RetryTheory` both support dynamically skipping tests at runtime. To make a test skip just use `Skip.Always()`
within your test code.  
It also supports custom exception types so you can skip a test if a type of exception gets thrown. You do this by specifying the exception type to the 
attribute above your test, e.g.
```cs
[RetryFact(typeof(TestException))]
public void CustomException_SkipsAtRuntime()
{
    throw new TestException();
}
```
This functionality also allows for skipping to work when you are already using another library for dynamically skipping tests by specifying the exception
type that is used by that library to the `RetryFact`. e.g. if you are using the popular Xunit.SkippableFact nuget package and want to add retries, converting the 
test is as simple as replacing `[SkippableFact]` with `[RetryFact(typeof(Xunit.SkipException))]` above the test and you don't need to change the test itself.

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