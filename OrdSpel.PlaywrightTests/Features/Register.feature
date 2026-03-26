Feature: Register

A short summary of the feature

@tag1
Scenario: [scenario name]
	Given [context]
	When [action]
	Then [outcome]

Scenario: Successful registration
    Given I am on the register page
    When I fill in username "testuser" and password "Test123!" and confirm the password
    Then I should be redirected to the login page

