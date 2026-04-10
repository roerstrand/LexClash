using Microsoft.Playwright;
using OrdSpel.PlaywrightTests.Helpers;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class LoginSteps
    {
        private readonly IPage _page;
        private readonly AuthHelper _authHelper;
        private readonly string _baseUrl;


        public LoginSteps(Hooks.Hooks hooks)
        {
            _page = hooks.Page;
            _baseUrl = hooks.BaseUrl;
            _authHelper = new AuthHelper(_page, _baseUrl);
        }

        [Given("I am on the login page")]
        public async Task GivenIAmOnTheLoginPage()
        {
            await _page.GotoAsync(_baseUrl);
        }

        [When(@"I fill in username ""(.*)"" and password ""(.*)""")]
        public async Task WhenIFillInUsernameAndPassword(string username, string password)
        {
            await _authHelper.LoginAsync(username, password);
        }

        [When("I submit an empty login form")]
        public async Task WhenISubmitAnEmptyLoginForm()
        {
            await _page.ClickAsync("button[type='submit']");
        }

        [Then("I should be redirected to the game page")]
        public async Task ThenIShouldBeRedirectedToTheGamePage()
        {
            await _page.WaitForURLAsync("**/game");
            Assert.That(_page.Url, Does.Contain("/game"));
        }

        [Then("I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            await _page.Locator("#errorBox")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var text = await _page.Locator("#errorBox").InnerTextAsync();
            Assert.That(text, Does.Contain("Fel användarnamn eller lösenord"));

        }

        [Then("Then I should see validation error")]
        public async Task ThenIShouldSeeValidationError()
        {
            await _page.Locator("#errorBox")
                .WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var text = await _page.Locator("#errorBox").InnerTextAsync();
            Assert.That(text, Does.Contain("Vänligen fyll i både användarnamn"));
        }
    }
}
