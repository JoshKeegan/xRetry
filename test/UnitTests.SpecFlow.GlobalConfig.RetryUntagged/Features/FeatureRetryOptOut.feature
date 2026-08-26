@retry(1)
Feature: Feature Retry Opt Out
	In order to keep retries opt-in for selected features
	As a QA engineer
	I want a feature retry tag to be able to opt out of retry by default

Scenario: Feature retry tag can opt out of retry by default
	When I increment the retry count
	Then the result should be 1
