Feature: Retry Scenarios
	In order to allow for transient failures
	As a QA engineer
	I want to be able to run scenario tests multiple times until they pass

@retry
Scenario: Retry three times by default
	When I increment the default retry count
	Then the default result should be 3

@retry(5)
Scenario: Retry five times when specified
	When I increment the retry count
	Then the result should be 5

@retry(2,100)
Scenario: Retry twice, waiting 100ms between retries
	When I start the stopwatch if not already started
	Then the elapsed time on the stopwatch is greater than or equal to 90ms

@ignore
Scenario: Test is ignored
	Then fail because this test should have been skipped

@ignore @retry
Scenario: Retry test is ignored
	Then fail because this test should have been skipped

@retry @ignore
Scenario: Retry test is ignored regardless of tag order
	Then fail because this test should have been skipped