# xRetry
Retry flickering test cases for xUnit and SpecFlow in dotnet core.

[![pipeline status](https://gitlab.com/JoshKeegan/xRetry/badges/master/pipeline.svg)](https://gitlab.com/JoshKeegan/xRetry/pipelines)

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
Add the `xRetry.SpecFlow` nuget package to your project.  

Above any scenario that should be retried, add a `@retry` tag, e.g:
```gherkin
@retry
Scenario: Retry three times by default
	When I increment the default retry count
	Then the default result should be 3
```
This will retry the test up to 3 times by default. You can optionally specify a number of times 
to retry the test in brackets, e.g. `@retry(5)`.  

You can also optionally specify a delay between each retry (in milliseconds) as a second 
parameter, e.g. `@retry(5,100)` will run your test 5 times until it passes, waiting 100ms
between each attempt.  
Note that you must not include a space between the parameters, as Cucumber/SpecFlow uses
a space to separate tags, i.e. `@retry(5, 100)` would not work due to the space after the comma.

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

You can also optionally specify a delay between each retry (in milliseconds) as a second 
parameter, e.g. `[RetryFact(5, 100)]` will run your test 5 times until it passes, waiting 100ms
between each attempt.

## Contributing
Feel free to open a pull request! If you want to start any sizeable chunk of work, consider 
opening an issue first to discuss, and make sure nobody else is working on the same problem.  

### Running locally
To run locally, always build `xRetry.SpecFlowPlugin` before the tests, to ensure MSBuild
uses the latest version of your changes.  

If you install `make` and go to the `build` directory, you can run the following from the 
terminal to build, run tests and create the nuget packages:
```bash
make build unit-tests-run nuget-create
```
If that works, all is well!

## Licence
[MIT](LICENSE)
