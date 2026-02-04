@retry(5)
Feature: Retry N Times Feature
In order to allow for transient failures on any test in a feature
As a QA engineer
I want to be able to run each scenario in a feature N times until they pass

    Scenario: Retry scenario five times when specified on the feature
        When I increment the retry count
        Then the result should be 5

    Scenario Outline: Retry scenario outline five times when specified on the feature
        When I increment the retry count for test <n>
        Then the retry count for test <n> should be 5

        Examples:
          | n |
          | 1 |
          | 2 |

    @retry
    Scenario: Parameters on the scenario take precedent: retry three times by default
        When I increment the retry count
        Then the result should be 3

    @retry
    Scenario Outline: Parameters on the scenario outline take precedent: retry three times by default
        When I increment the retry count for test <n>
        Then the retry count for test <n> should be 3

        Examples:
          | n |
          | 1 |
          | 2 |