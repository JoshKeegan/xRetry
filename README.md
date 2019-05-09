# xRetry
Retry flickering test cases for xUnit and SpecFlow in dotnet core.

[![pipeline status](https://gitlab.com/JoshKeegan/SpecflowXunitRetry/badges/master/pipeline.svg)](https://gitlab.com/JoshKeegan/SpecflowXunitRetry/commits/master)

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

## Usage: SpecFlow 3
Add the `xRetry.SpecFlowPlugin` nuget package to your project.  

Above any scenario that should be retried, add a `@retry` tag, e.g:
```gherkin
@retry
Scenario: Retry three times by default
	When I increment the default retry count
	Then the default result should be 3
```
This will retry the test up to 3 times by default. You can optionally specify a number of times 
to retry the test in brackets, e.g. `@retry(5)`.

## Usage: xUnit
Add the `xRetry` nuget package to your project.

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
This will retry the test up to 3 times by default. You can optionally specify a number of times
to retry the test as an argument, e.g. `[RetryFact(5)]`.

## Licence
[MIT](LICENSE)