[//]: # (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)


[//]: \# (Src: xRetry.Reqnroll.v3/header.md)

# xRetry.Reqnroll.v3
Retry flickering test cases for Reqnroll when using xUnit v3.

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

[//]: \# (Src: xRetry.Reqnroll.v3/usage.md)

## Usage
Add the `xRetry.Reqnroll.v3` nuget package to your project.

Above any scenario or scenario outline that should be retried, add a `@retry` tag, e.g:
```gherkin
@retry
Scenario: Retry three times by default
	Given I have a default case
	When it fails
	Then it should be retried three times
```

This will attempt to run the test until it passes, up to 3 times by default.

You can also optionally specify a number of times to attempt to run the test in brackets, e.g. `@retry(5)`.
```gherkin
@retry(5)
Scenario: Retry five times
	Given I have a case with a retry count
	When it fails
	Then it should be retried five times
```

You can also optionally specify a delay between each retry (in milliseconds) as a second
parameter, e.g. `@retry(5,100)` will run your test 5 times until it passes, waiting 100ms
between each attempt.
```gherkin
@retry(5,100)
Scenario: Retry five times with a 100ms delay
	Given I have a case with a retry count and delay
	When it fails
	Then it should be retried five times with a 100ms delay
```

Note: These retry tags can also be applied to features, in which case every test in the feature
will be retried. Retry tags on a scenario will take precedence over a retry tag applied to the
feature the scenario is within.

## Differences from xRetry.Reqnroll

This package (`xRetry.Reqnroll.v3`) is for use with **xUnit v3** via `Reqnroll.xunit.v3`.

If you are using xUnit v2 with `Reqnroll.xUnit`, use the `xRetry.Reqnroll` package instead.
