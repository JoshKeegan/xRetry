Feature: Package smoke test

Scenario: An untagged scenario uses runtime configuration from the packed package
	When the package smoke scenario is attempted
	Then it succeeds on the second attempt
