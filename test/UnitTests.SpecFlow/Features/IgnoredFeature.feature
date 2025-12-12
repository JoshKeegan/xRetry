@ignore
Feature: Ignored Feature
	In order to temporarily disable features containing retryable scenarios
	As a QA engineer
	I want to be able to ignore entire features that contain retryable tests

Scenario: Test is ignored
	Then fail because this test should have been skipped

@retry
Scenario: Retry test is ignored
	Then fail because this test should have been skipped

Scenario Outline: Scenario outline test is ignored
	Then fail because this test should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |

@retry
Scenario Outline: Retry scenario outline test is ignored
	Then fail because this test should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |