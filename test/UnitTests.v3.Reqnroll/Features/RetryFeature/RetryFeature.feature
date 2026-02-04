@retry
Feature: Retry Feature
In order to allow for transient failures on any test in a feature
As a QA engineer
I want to be able to run each scenario in a feature multiple times until they pass

    Scenario: Retry scenario three times by default
        When I increment the retry count
        Then the result should be 3

    @ignore
    Scenario: Scenario is ignored
        Then fail because this test should have been skipped

    Scenario Outline: Retry scenario outline three times by default
        When I increment the retry count for test <n>
        Then the retry count for test <n> should be 3

        Examples:
          | n |
          | 1 |
          | 2 |

    @ignore
    Scenario Outline: Scenario outline is ignored
        Then fail because this test <n> should have been skipped

        Examples:
          | n |
          | 1 |
          | 2 |

    @retry(5)
    Scenario: Parameters on the scenario take precedent: retry scenario five times when specified
        When I increment the retry count
        Then the result should be 5

    @retry(5)
    Scenario Outline: Parameters on the scenario take precedent: retry scenario outline five times when specified
        When I increment the retry count for test <n>
        Then the retry count for test <n> should be 5

        Examples:
          | n |
          | 1 |
          | 2 |