@retry
Feature: Global Retry Scenarios Outlines
	In order to allow for transient failures
	As a QA engineer
	I want to be able to run scenario tests multiple times until they pass

Scenario Outline: Retry implicit three times by default
	When I increment the global outline retry count for test <n>
	Then the global outline retry could for test <n> should be 3
	Examples:
	| n |
	| 1 |
	| 2 |

