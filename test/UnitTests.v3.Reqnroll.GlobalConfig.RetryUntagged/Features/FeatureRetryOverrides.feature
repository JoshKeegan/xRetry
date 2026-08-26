@retry(5)
Feature: Feature Retry Overrides
	In order to customise retry behaviour within a feature
	As a QA engineer
	I want feature and scenario retry tags to override the configured defaults in order

Scenario: Feature retry tag overrides configured max retries
	When I increment the retry count
	Then the result should be 5

@retry(6)
Scenario: Scenario retry tag overrides the feature retry tag
	When I increment the retry count
	Then the result should be 6

@retry(1)
Scenario: Scenario retry tag can opt out of retries
	When I increment the retry count
	Then the result should be 1
