using Microsoft.Playwright;
using OrdSpel.PlaywrightTests.Helpers;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions;

[Binding]
public class GameResultSteps
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    private readonly AuthHelper _authHelper;

    public GameResultSteps(Hooks.Hooks hooks)
    {
        _page = hooks.Page;
        _baseUrl = hooks.BaseUrl;
        _authHelper = new AuthHelper(_page, _baseUrl);
    }

    [Given(@"I am logged in as ""(.*)"" with password ""(.*)""")]
    public async Task GivenIAmLoggedInAsWithPassword(string username, string password)
    {
        await _authHelper.LoginAsync(username, password);
    }

    [When("I navigate to {string}")]
    public async Task WhenINavigateTo(string relativePath)
    {
        await _page.GotoAsync($"{_baseUrl}{relativePath}");
    }

    [Then("I should see the game result load error message")]
    public async Task ThenIShouldSeeTheGameResultLoadErrorMessage()
    {
        await _page.WaitForSelectorAsync("#gameResultError");
        var text = await _page.TextContentAsync("#gameResultError");
        Assert.That(text, Does.Contain("Kunde inte hämta resultat"));
    }
}
