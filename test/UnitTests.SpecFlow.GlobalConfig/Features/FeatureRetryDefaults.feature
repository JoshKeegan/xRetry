@retry
Feature: Feature Retry Defaults
	In order to keep retries opt-in when config is present
	As a QA engineer
	I want feature retry tags to use configured defaults without enabling retry by default

Scenario: Feature retry tag uses configured max retries
	When I increment the retry count
	Then the result should be 4

Scenario: Feature retry tag uses configured delay
	When I start the stopwatch if not already started
	Then the elapsed time on the stopwatch is greater than or equal to 120ms

@retry(1)
Scenario: Scenario retry tag can opt out of the feature retry tag
	When I increment the retry count
	Then the result should be 1
