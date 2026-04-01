Feaure: Login

Summary

@tag1
Scenario: [scenario name]
	Given [context]
	When [action]
	Then [outcome]

	Scenario: Successful login
		Given I am on the login page
		WHen I fill in username "testuser" and password "Test123!"
		Then I should be redirected to the game page

	Scenario: Failed login with wrong password
		Given I am on the login page
		When I fill in username "testuser" and password "wrong"
		Then I should see an error message

	Scenario: Login with empty fields
		Given I am on the login page
		When I submit empty login form
		Then I should see validation error
