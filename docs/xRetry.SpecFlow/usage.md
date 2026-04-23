## Usage: SpecFlow 3

Add the [`xRetry.SpecFlow` NuGet package](https://www.nuget.org/packages/xRetry.SpecFlow "xRetry NuGet.SpecFlow package") to your project.  

### Scenarios (and outlines)

Above any scenario or scenario outline that should be retried, add a `@retry` tag, e.g:

```gherkin
@retry
Scenario: Retry three times by default
	When I increment the default retry count
	Then the default result should be 3
```

This will attempt to run the test until it passes, up to 3 times by default. 
You can optionally specify a number of times to attempt to run the test in brackets, e.g. `@retry(5)`.  

You can also optionally specify a delay between each retry (in milliseconds) as a second
parameter, e.g. `@retry(5,100)` will run your test up to 5 times, waiting 100ms between each attempt.  
Note that you must not include a space between the parameters, as Cucumber/SpecFlow uses
a space to separate tags, i.e. `@retry(5, 100)` would not work due to the space after the comma.

### Features

You can also make every test in a feature retryable by adding the `@retry` tag to the feature, e.g:

```gherkin
@retry
Feature: Retryable Feature

Scenario: Retry scenario three times by default
	When I increment the retry count
	Then the result should be 3
```

All options that can be used against an individual scenario can also be applied like this at the feature level.  
If a `@retry` tag exists on both the feature and a scenario within that feature, the tag on the scenario will take
precedent over the one on the feature. This is useful if you wanted all scenarios in a feature to be retried
by default but for some cases also wanted to wait some time before each retry attempt. You can also use this to prevent a specific scenario not be retried, even though it is within a feature with a `@retry` tag, by adding `@retry(1)` to the scenario.

### Project-wide defaults

You can define project-wide defaults in a file named `xretry.json` in the root of your test project directory:

```json
{
  "maxRetries": 4,
  "delayBetweenRetriesMs": 50,
  "retryUntaggedScenarios": true
}
```

Supported keys:
 - `maxRetries`: default maximum number of attempts
 - `delayBetweenRetriesMs`: default delay between attempts, in milliseconds
 - `retryUntaggedScenarios`: when `true`, untagged scenarios use the configured or built-in retry defaults

To make the same file available when the generated xUnit tests run, copy it to the output directory, e.g:

```xml
<ItemGroup>
    <Content Include="xretry.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

By default, retries remain opt-in and only scenarios or features tagged with `@retry` are retried.

If `retryUntaggedScenarios` is `true`, untagged scenarios use the configured defaults:

```gherkin
Feature: Retryable Feature

Scenario: Uses global defaults
	When I increment the retry count
	Then the result should be 4
```

Precedence is:
 - scenario `@retry` tag
 - feature `@retry` tag
 - `xretry.json` when `retryUntaggedScenarios` is `true`
 - no retries for untagged scenarios otherwise

Any value not specified in a tag falls back to the next level. For example, plain `@retry` uses the configured
retry count and delay when present, `@retry(5)` overrides the retry count but can still use the configured delay,
and `@retry(1)` can be used to opt out even when a feature tag or `retryUntaggedScenarios` would otherwise retry
the scenario.

If the same test project also contains direct xUnit retries via `xRetry`, those tests read the same `xretry.json` too.
Direct xUnit retries still require `[RetryFact]` or `[RetryTheory]`; `retryUntaggedScenarios` only affects the
SpecFlow-generated tests.
