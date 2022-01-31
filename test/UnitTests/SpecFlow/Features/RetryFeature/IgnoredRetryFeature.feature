@retry @ignore
Feature: Ignored Retriable Feature
	In order to temorarily disable retriable features
	As a QA engineer
	I want to be able to ignore entire features marked for retries

Scenario: Test is ignored
	Then fail because this test should have been skipped

@retry
Scenario: Explicit retry test is ignored
	Then fail because this test should have been skipped

Scenario Outline: Scenario outline test is ignored
	Then fail because this test should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |

@retry
Scenario Outline: Explicit retry scenario outline test is ignored
	Then fail because this test should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |