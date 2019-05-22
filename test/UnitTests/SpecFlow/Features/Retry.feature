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
