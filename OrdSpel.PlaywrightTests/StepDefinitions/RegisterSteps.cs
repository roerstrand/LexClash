using Microsoft.Playwright;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class RegisterSteps
    {
        private readonly IPage _page;

        public RegisterSteps(Hooks.Hooks hooks)
        {
            _page = hooks.Page;
        }

        [Given("I am on the register page")]
        public async Task GivenIAmOnTheRegisterPage()
        {
            await _page.GotoAsync("https://localhost:PORT/register"); //Byt ut!!!
        }

        [When("I fill in username {string} and password {string} and confirm the password")]
        public async Task WhenIFillInUsernameAndPassword(string username, string password)
        {
            await _page.FillAsync("input[type='text']", username);
            await _page.FillAsync("input[type='password']", password);
            await _page.FillAsync("input[type='password']:nth-of-type(2)", password);
            await _page.ClickAsync("button[type='submit']");
        }

        [Then("I should be redirected to the login page")]
        public async Task ThenIShouldBeRedirectedToTheLoginPage()
        {
            await _page.WaitForURLAsync("**/login");
            Assert.That(_page.Url, Does.Contain("/login"));
        }
    }
}
