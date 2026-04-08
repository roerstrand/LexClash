using Microsoft.Playwright;
using Reqnroll;

namespace OrdSpel.PlaywrightTests.StepDefinitions;

[Binding]
public class GameResultSteps
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public GameResultSteps(Hooks.Hooks hooks)
    {
        _page = hooks.Page;
        _baseUrl = hooks.BaseUrl;
    }

    [Then("I should see the game result load error message")]
    public async Task ThenIShouldSeeTheGameResultLoadErrorMessage()
    {
        await _page.WaitForSelectorAsync("#gameResultError");
        var text = await _page.TextContentAsync("#gameResultError");
        Assert.That(text, Does.Contain("Kunde inte hämta resultat"));
    }
}
