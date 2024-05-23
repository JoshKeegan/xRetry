Feature: Retry Ignore Scenarios
	In order to allow for tests to be ignored/skipped at runtime
	So that the full feature set of Reqnroll is still available with xRetry (IUnitTestRuntimeProvider.TestIgnore)
	As a QA engineer
	I want to be able to ignore/skip tests

@retry
Scenario: Test is ignored at runtime
	When I ignore this test
	Then fail because this test should have been skipped

@retry
Scenario Outline: Test (outline) is ignored at runtime
	When I ignore this test
	Then fail because this test <n> should have been skipped
	Examples: 
	| n |
	| 1 |
	| 2 |