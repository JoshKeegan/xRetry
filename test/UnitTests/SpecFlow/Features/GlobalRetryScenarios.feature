@retry
Feature: Global Retry Scenarios
	In order to allow for transient failures
	As a QA engineer
	I want to be able to run scenario tests multiple times until they pass

Scenario: Retry implicit three times by default
	When I increment the global retry count
	Then the global result should be 3
