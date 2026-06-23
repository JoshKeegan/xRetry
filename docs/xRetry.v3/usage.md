## Usage: xUnit v3

Add the [`xRetry.v3` NuGet package](https://www.nuget.org/packages/xRetry.v3 "xRetry.v3 NuGet package") to your project.

### Facts

Above any `Fact` test case that should be retried, replace the `Fact` attribute, with
`RetryFact`, e.g:

```cs
using xRetry.v3;

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

If the same test project also uses `xRetry.v3.Reqnroll`, it reads the same `xretry.json` too. That BDD integration keeps
retries opt-in by default and only applies the config to untagged scenarios when `retryUntaggedScenarios` is set to `true`.

### Skipping tests at Runtime

[xUnit.v3 has added native support for skipping tests at runtime](https://xunit.net/docs/getting-started/v3/whats-new#dynamically-skippable-tests).
If you were using the xRetry skipping functionality with xUnit v2, you should find upgrading simple.

Most use cases will be covered by replacing `Skip.Always();` with `Assert.Skip("your reason");`.

If you are skipping custom exceptions, you will also need to change the way they are passed to the test attributes:  
`[RetryFact(typeof(TestException))]` would now be `[RetryFact(SkipExceptions = new[] {typeof(TestException)})]`  
`[RetryTheory(typeof(TestException))]` would now be `[RetryTheory(SkipExceptions = new[] {typeof(TestException)})]`
