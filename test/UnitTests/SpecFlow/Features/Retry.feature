Feature: Retry
	In order to allow for transient failures
	As a QA engineer
	I want to be able to run tests multiple times until they pass

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
	Then the stopwatch elapsed milliseconds is greater than or equal to '90'