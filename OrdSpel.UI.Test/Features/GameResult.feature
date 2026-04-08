Feature: Game result

Scenario: Game result page shows error for unknown game code
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to "/game/ZZZZZZ/result"
    Then I should see the game result load error message
