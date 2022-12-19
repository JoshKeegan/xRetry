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