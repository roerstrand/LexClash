using System.Text.RegularExpressions;
using Microsoft.Playwright;
using OrdSpel.PlaywrightTests.Helpers;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions;

[Binding]
public class LobbySteps
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    private readonly AuthHelper _authHelper;

    public LobbySteps(Hooks.Hooks hooks)
    {
        _page = hooks.Page;
        _baseUrl = hooks.BaseUrl;
        _authHelper = new AuthHelper(_page, _baseUrl);
    }

    [Scope(Tag = "Lobby")]

    [Given(@"I log in as ""(.*)"" with password ""(.*)""")]
    [Given(@"I am logged in as ""(.*)"" with password ""(.*)""")]
    public async Task GivenIAmLoggedInAsWithPassword(string username, string password)
    {
        await _authHelper.LoginAsync(username, password);
    }

    // The "I select a category and click Create" step is implemented in CreateJoinGameSteps.cs

    [When("I navigate to {string}")]
    public async Task WhenINavigateTo(string relativePath)
    {
        // Build a stable absolute URL and navigate.
        var baseUri = new Uri(_baseUrl);
        var trimmed = relativePath?.TrimStart('/') ?? string.Empty;
        var target = new Uri(baseUri, trimmed).ToString();

        try
        {
            await _page.GotoAsync(target, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 60000 });
        }
        catch
        {
            try
            {
                var path = $"lobby_navigate_failure_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    [Then("I should be on a lobby page")]
    public async Task ThenIShouldBeOnALobbyPage()
    {
        // Lobby is a component rendered on /game/{code} route.
        // Wait for a lobby-specific element to appear instead of relying on URL.
        await _page.WaitForSelectorAsync("#copyGameCodeButton", new PageWaitForSelectorOptions { Timeout = 60000 });
        await _page.WaitForSelectorAsync("#lobbyCategory", new PageWaitForSelectorOptions { Timeout = 60000 });
    }

    [Then("I should see the lobby game code")]
    public async Task ThenIShouldSeeTheLobbyGameCode()
    {
        await _page.WaitForSelectorAsync("#copyGameCodeButton", new PageWaitForSelectorOptions { Timeout = 60000 });
        // The page shows the label "Spelkod" (without colon) - wait for that text with an extended timeout
        await _page.WaitForSelectorAsync("text=Spelkod", new PageWaitForSelectorOptions { Timeout = 60000 });
    }

    [Then("I should see lobby details")]
    public async Task ThenIShouldSeeLobbyDetails()
    {
        await _page.WaitForSelectorAsync("#lobbyStatus", new PageWaitForSelectorOptions { Timeout = 60000 });
        await _page.WaitForSelectorAsync("#lobbyPlayers", new PageWaitForSelectorOptions { Timeout = 60000 });
        await _page.WaitForSelectorAsync("#lobbyCategory", new PageWaitForSelectorOptions { Timeout = 60000 });
        await _page.WaitForSelectorAsync("#lobbyStartWord", new PageWaitForSelectorOptions { Timeout = 60000 });
    }

   

   
}
