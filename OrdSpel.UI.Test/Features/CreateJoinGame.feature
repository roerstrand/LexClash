Feature: Create and join game

Scenario: Logged in user can create a game and see the game code
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to the game page
    And I select a category and click Create
    Then I should see a game code on the screen

Scenario: User sees error message when entering invalid game code
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to the game page
    And I enter the game code "000000" and click Join
    Then I should see the error message "Spelet hittades inte."
