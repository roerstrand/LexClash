Feature: Active game view

Scenario: Active game view shows game elements during ongoing game
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to the game page
    And I select a category and click Create
    And I note the game code from the lobby URL
    And a second player joins the game via API
    Then I should be on the active game page
    And I should see the current word
    And I should see the required letter
    And I should see the scoreboard
