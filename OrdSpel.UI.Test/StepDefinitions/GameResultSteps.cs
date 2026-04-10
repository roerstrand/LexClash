//using System;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Text;
//using System.Text.Json;
//using System.Text.RegularExpressions;
//using Microsoft.Playwright;
//using OrdSpel.PlaywrightTests.Helpers;
//using Reqnroll;

//namespace OrdSpel.PlaywrightTests.StepDefinitions;

//[Binding]
//public class GameResultSteps
//{
//    private readonly IPage _page;
//    private readonly string _baseUrl;
//    private readonly AuthHelper _authHelper;

//    public GameResultSteps(Hooks.Hooks hooks)
//    {
//        _page = hooks.Page;
//        _baseUrl = hooks.BaseUrl;
//        _authHelper = new AuthHelper(_page, _baseUrl);
//    }

//    [When("I create and finish a game via API")]
//    public async Task WhenICreateAndFinishAGameViaApi()
//    {
//        // Retrieve Cookies for API Authentication
//        var cookies = await _page.Context.CookiesAsync();
//        var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));

//        using var client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
//        if (!string.IsNullOrEmpty(cookieHeader)) client.DefaultRequestHeaders.Add("Cookie", cookieHeader);

//        // Find one category to use for game creation
//        var catResp = await client.GetAsync("api/category");
//        catResp.EnsureSuccessStatusCode();
//        var cats = await catResp.Content.ReadFromJsonAsync<List<dynamic>>() ?? new List<dynamic>();
//        if (cats.Count == 0) throw new InvalidOperationException("No categories available");
//        int categoryId = (int)(cats[0].Id ?? cats[0].id);

//        // Create game
//        var payload = JsonSerializer.Serialize(new { CategoryId = categoryId });
//        var createResp = await client.PostAsync("api/game/create", new StringContent(payload, Encoding.UTF8, "application/json"));
//        createResp.EnsureSuccessStatusCode();
//        var created = await createResp.Content.ReadFromJsonAsync<OrdSpel.Shared.GameDTOs.GameSessionResponseDto>();
//        var gameCode = created?.GameCode ?? throw new InvalidOperationException("Create returned no gameCode");

//        // EndGame
//        var endResp = await client.PutAsync($"api/game/end/{gameCode}", null);
//        endResp.EnsureSuccessStatusCode();

//        // Navigate to game result page
//        await _page.GotoAsync(new Uri(new Uri(_baseUrl), $"game/{gameCode}").ToString(), new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
//        await _page.WaitForSelectorAsync("#gameResultTitle", new PageWaitForSelectorOptions { Timeout = 60000 });
//    }

//    [Then("I should see the game result")]
//    public async Task ThenIShouldSeeTheGameResult()
//    {
//        await _page.WaitForSelectorAsync("#gameResultTitle", new PageWaitForSelectorOptions { Timeout = 60000 });
//        await _page.WaitForSelectorAsync("#gameResultScoreboard", new PageWaitForSelectorOptions { Timeout = 60000 });
//    }
//}
