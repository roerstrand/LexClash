Feature: Lobby room

Scenario: Created game shows lobby details
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to "/game"
    When I select a category and click Create
    Then I should be on a lobby page
    And I should see the lobby game code
    And I should see lobby details

Scenario: Lobby without code shows link to game page
    Given I am logged in as "playwright_user" with password "Test123!"
    When I navigate to "/lobby"
    Then I should see the message "Ingen spelkod hittades."
