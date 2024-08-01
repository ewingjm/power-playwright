Feature: Login

    The `ModelDrivenApp` class provides login functionality.

@formId:e32b498d-c577-48ef-b125-238cd169eb2b
Scenario: Login with credentials
	Given I have instantiated a ModelDrivenApp instance
	When I call the LoginAsync method
	Then I will be logged in with the provided credentials

Scenario: Login with cached token
	Given I have instantiated a ModelDrivenApp instance
	And I have called the LoginAsync method
	And I have instantiated a ModelDrivenApp instance with the same credentials
	When I call the LoginAsync method
	Then I will bypass the login