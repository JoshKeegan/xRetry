@ignore
Feature: Ignored Feature
	In order to temorarily disable retriable features
	As a QA engineer
	I want to be able to ignore entire features that contain retriable tests

@retry
Scenario: Retry test is ignored
	Then fail because this test should have been skipped