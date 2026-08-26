Feature: Global Retry Defaults
	In order to retry scenarios by default
	As a QA engineer
	I want untagged scenarios to use the configured global defaults

Scenario: Untagged scenario uses configured max retries
	When I increment the retry count
	Then the result should be 4

Scenario: Untagged scenario uses configured delay
	When I start the stopwatch if not already started
	Then the elapsed time on the stopwatch is greater than or equal to 120ms

@retry(1)
Scenario: Scenario retry tag can opt out of retry by default
	When I increment the retry count
	Then the result should be 1

@ignore
Scenario: Ignored scenario is skipped and not retried
	Then fail because this test should have been skipped

Scenario Outline: Scenario outline row ignore is skipped and not retried
	When I increment the retry count
	Then the result should be <expected>
	Examples:
	| expected |
	| 4        |

	@ignore
	Examples: Ignored row
	| expected |
	| 999      |

@retry(5)
Scenario: Scenario retry tag overrides configured max retries
	When I increment the retry count
	Then the result should be 5

@retry(5,100)
Scenario: Scenario retry tag overrides configured delay
	When I start the stopwatch if not already started
	Then the elapsed time on the stopwatch is greater than or equal to 350ms
