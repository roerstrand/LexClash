@Result
Feature: Game result

Scenario: Finished game shows results
    Given I am logged in as "playwright_user" with password "Test123!"
    When I create and finish a game via API
    Then I should see the game result
