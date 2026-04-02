using Microsoft.Playwright;
using OrdSpel.PlaywrightTests.Helpers;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class CreateJoinGameSteps
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

        public CreateJoinGameSteps(Hooks.Hooks hooks)
        {
            _page = hooks.Page;
            _baseUrl = hooks.BaseUrl;
        }

        [Given("I am logged in as {string} with password {string}")]
        public async Task GivenIAmLoggedInAs(string username, string password)
        {
            var authHelper = new AuthHelper(_page, _baseUrl);
            await authHelper.LoginAsync(username, password);
        }

        [When("I navigate to the game page")]
        public async Task WhenINavigateToTheGamePage()
        {
            await Task.Delay(2000);
        }

        [When("I select a category and click Create")]
        public async Task WhenISelectACategoryAndClickCreate()
        {
            await _page.WaitForFunctionAsync("() => document.querySelector('#categorySelect')?.options.length > 1");
            await _page.SelectOptionAsync("#categorySelect", new[] { new SelectOptionValue { Index = 1 } });
            await _page.ClickAsync("#createGameButton");
        }

        [Then("I should see a game code on the screen")]
        public async Task ThenIShouldSeeAGameCodeOnTheScreen()
        {
            // Efter att spelet skapats navigeras användaren till /lobby/{spelkod}
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex(@"/lobby/[A-Za-z0-9]{6}$"),
                new PageWaitForURLOptions { Timeout = 10000 });

            var url = _page.Url;
            var gameCode = url.Split("/lobby/").Last();
            Assert.That(gameCode, Is.Not.Null.And.Not.Empty);
            Assert.That(gameCode.Length, Is.EqualTo(6));
        }

        [When("I enter the game code {string} and click Join")]
        public async Task WhenIEnterTheGameCodeAndClickJoin(string gameCode)
        {
            await _page.FillAsync("#gameCodeInput", gameCode);
            await _page.ClickAsync("#joinGameButton");
        }

        [Then("I should see the error message {string}")]
        public async Task ThenIShouldSeeTheErrorMessage(string errorMessage)
        {
            await _page.WaitForSelectorAsync("#joinError");
            var error = await _page.TextContentAsync("#joinError");
            Assert.That(error, Does.Contain(errorMessage));
        }
    }
}
