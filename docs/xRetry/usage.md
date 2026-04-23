## Usage: xUnit

Add the [`xRetry` NuGet package](https://www.nuget.org/packages/xRetry "xRetry NuGet package") to your project.

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
`MaxRetries` and `DelayBetweenRetriesMs` can also be set individually as attribute properties, e.g. `[RetryFact(DelayBetweenRetriesMs = 100)]`.

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

### Project-wide defaults

You can define project-wide defaults in a file named `xretry.json` in the root of your test project directory:

```json
{
  "maxRetries": 5,
  "delayBetweenRetriesMs": 100
}
```

Supported keys:
 - `maxRetries`: default maximum number of attempts
 - `delayBetweenRetriesMs`: default delay between attempts, in milliseconds

If the file or a key is missing, xRetry falls back to the built-in defaults of `3` retries and `0ms` delay.

To make the file available when the tests run, copy it to the output directory, e.g:

```xml
<ItemGroup>
    <Content Include="xretry.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

With this file in place, `[RetryFact]` and `[RetryTheory]` use the configured defaults whenever an argument is not supplied.
Explicit attribute values take precedence over the config, and any value not specified explicitly falls back to the config
and then the built-in defaults. For example, `[RetryFact(5)]` uses `5` attempts and still uses the configured delay.

If the same test project also uses `xRetry.SpecFlow` or `xRetry.Reqnroll`, they read the same `xretry.json` too. Those
BDD integrations keep retries opt-in by default and only apply the config to untagged scenarios when
`retryUntaggedScenarios` is set to `true`.

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
type that is used by that library to the `RetryFact`. e.g. if you are using the popular Xunit.SkippableFact NuGet package and want to add retries, converting the
test is as simple as replacing `[SkippableFact]` with `[RetryFact(typeof(Xunit.SkipException))]` above the test and you don't need to change the test itself.
