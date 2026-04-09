using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions;

[Binding]
public class LobbySteps
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public LobbySteps(Hooks.Hooks hooks)
    {
        _page = hooks.Page;
        _baseUrl = hooks.BaseUrl;
    }

    [When("I navigate to {string}")]
    public async Task WhenINavigateTo(string relativePath)
    {
        await _page.GotoAsync($"{_baseUrl}{relativePath}");
    }

    [Then("I should be on a lobby page")]
    public async Task ThenIShouldBeOnALobbyPage()
    {
        await _page.WaitForURLAsync(new Regex(@"/game/[A-Za-z0-9]{6}$"), new PageWaitForURLOptions { Timeout = 10000 });
    }

    [Then("I should see the lobby game code")]
    public async Task ThenIShouldSeeTheLobbyGameCode()
    {
        await _page.WaitForSelectorAsync("#copyGameCodeButton");
        await _page.WaitForSelectorAsync("text=Spelkod:");
    }

    [Then("I should see lobby details")]
    public async Task ThenIShouldSeeLobbyDetails()
    {
        await _page.WaitForSelectorAsync("#lobbyStatus");
        await _page.WaitForSelectorAsync("#lobbyPlayers");
        await _page.WaitForSelectorAsync("#lobbyCategory");
        await _page.WaitForSelectorAsync("#lobbyStartWord");
    }

    [Then("I should see the message {string}")]
    public async Task ThenIShouldSeeTheMessage(string message)
    {
        await _page.WaitForSelectorAsync("#lobbyNoCodeMessage");
        var actual = await _page.TextContentAsync("#lobbyNoCodeMessage");
        Assert.That(actual, Does.Contain(message));
    }

}
