Feature: Tagged Scenario Retry Defaults
	In order to keep retries opt-in when config is present
	As a QA engineer
	I want untagged scenarios to remain un-retried and tagged scenarios to use configured defaults

Scenario: Untagged scenario is not retried when retry by default is disabled
	When I increment the retry count
	Then the result should be 1

@retry
Scenario: Scenario retry tag uses configured max retries
	When I increment the retry count
	Then the result should be 4

@retry
Scenario: Scenario retry tag uses configured delay
	When I start the stopwatch if not already started
	Then the elapsed time on the stopwatch is greater than or equal to 120ms
