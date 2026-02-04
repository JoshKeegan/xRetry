@retry(2,100)
Feature: Retry N Times With Delay Feature
In order to allow for transient failures on any test in a feature
As a QA engineer
I want to be able to run each scenario in a feature N times, with a delay between each attempt until they pass

    Scenario: Retry scenario twice, waiting 100ms between retries when specified on the feature
        When I start the stopwatch if not already started
        Then the elapsed time on the stopwatch is greater than or equal to 90ms

    Scenario Outline: Retry scenario outline twice, waiting 100ms between retries when specified on the feature
        When I start the stopwatch for test <n> if not already started
        Then the elapsed time on the stopwatch for test <n> is greater than or equal to 90ms

        Examples:
          | n |
          | 1 |
          | 2 |