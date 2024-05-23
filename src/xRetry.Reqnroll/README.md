[//]: # (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)


[//]: \# (Src: xRetry.Reqnroll/header.md)

# xRetry.Reqnroll
Retry flickering test cases for Reqnroll when using xUnit.

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

[//]: \# (Src: xRetry.Reqnroll/usage.md)

## Usage: Reqnroll 3

Add the [`xRetry.Reqnroll` NuGet package](https://www.nuget.org/packages/xRetry.Reqnroll "xRetry NuGet.Reqnroll package") to your project.  

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
Note that you must not include a space between the parameters, as Cucumber/Reqnroll uses
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