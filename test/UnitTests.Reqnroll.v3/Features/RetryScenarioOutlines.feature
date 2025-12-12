Feature: Retry Scenario Outlines
	In order to allow for transient failures
	As a QA engineer
	I want to be able to run scenario outline tests multiple times until they pass

@retry
Scenario Outline: Retry three times by default
	When I increment the retry count for test <n>
	Then the retry count for test <n> should be 3
	Examples:
	| n |
	| 1 |
	| 2 |

@retry(5)
Scenario Outline: Retry five times when specified
	When I increment the retry count for test <n>
	Then the retry count for test <n> should be 5
	Examples:
	| n |
	| 1 |
	| 2 |

@retry(2,100)
Scenario Outline: Retry twice, waiting 100ms between retries
	When I start the stopwatch for test <n> if not already started
	Then the elapsed time on the stopwatch for test <n> is greater than or equal to 90ms
	Examples:
	| n |
	| 1 |
	| 2 |

@ignore
Scenario Outline: Test is ignored
	Then fail because this test <n> should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |

@ignore @retry
Scenario Outline: Retry test is ignored
	Then fail because this test <n> should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |

@retry @ignore
Scenario Outline: Retry test is ignored regardless of tag order
	Then fail because this test <n> should have been skipped
	Examples:
	| n |
	| 1 |
	| 2 |
