using Microsoft.Playwright;
using Reqnroll;
using System.Text;
using System.Text.Json;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class ActiveGameSteps
    {
        private readonly IPage _page;
        private readonly string _apiBaseUrl = "https://localhost:7099";
        private readonly string _secondPlayerUsername = "activegame_player2";
        private readonly string _secondPlayerPassword = "Test123!";
        private string? _gameCode;

        public ActiveGameSteps(Hooks.Hooks hooks)
        {
            _page = hooks.Page;
        }

        [AfterScenario]
        public async Task Cleanup()
        {
            if (_gameCode == null) return;
            try
            {
                var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
                using var client = new HttpClient(handler);
                var loginBody = JsonSerializer.Serialize(new { username = _secondPlayerUsername, password = _secondPlayerPassword });
                await client.PostAsync($"{_apiBaseUrl}/api/auth/login",
                    new StringContent(loginBody, Encoding.UTF8, "application/json"));
                await client.DeleteAsync($"{_apiBaseUrl}/api/auth/delete");
            }
            catch { }
        }

        [When("I note the game code from the lobby URL")]
        public async Task WhenINoteTheGameCodeFromTheLobbyUrl()
        {
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex(@"/game/[A-Za-z0-9]{6}"),
                new PageWaitForURLOptions { Timeout = 10000 });
            var url = _page.Url;
            _gameCode = url.Split("/game/").Last().Split("?").First();
            Assert.That(_gameCode, Has.Length.EqualTo(6));
        }

        [When("a second player joins the game via API")]
        public async Task WhenASecondPlayerJoinsTheGameViaApi()
        {
            // Vänta på att SignalR-anslutningen i webbläsaren hinner koppla upp och joina spelgruppen
            // innan vi skickar join-eventet, annars missar lobbyn LobbyUpdated-notifikationen
            await Task.Delay(3000);

            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
            using var client = new HttpClient(handler);

            // Försök registrera – om användaren redan finns ignoreras felet
            var registerBody = JsonSerializer.Serialize(new { username = _secondPlayerUsername, password = _secondPlayerPassword, confirmPassword = _secondPlayerPassword });
            await client.PostAsync($"{_apiBaseUrl}/api/auth/register",
                new StringContent(registerBody, Encoding.UTF8, "application/json"));

            // Logga in – API:t sätter auth-cookie automatiskt, HttpClientHandler skickar den vidare
            var loginBody = JsonSerializer.Serialize(new { username = _secondPlayerUsername, password = _secondPlayerPassword });
            var loginResponse = await client.PostAsync($"{_apiBaseUrl}/api/auth/login",
                new StringContent(loginBody, Encoding.UTF8, "application/json"));

            Assert.That(loginResponse.IsSuccessStatusCode, Is.True, "Second player login failed");

            // Joina spelet – cookien skickas automatiskt
            var joinBody = JsonSerializer.Serialize(new { gameCode = _gameCode });
            var joinResponse = await client.PostAsync($"{_apiBaseUrl}/api/Game/join",
                new StringContent(joinBody, Encoding.UTF8, "application/json"));

            Assert.That(joinResponse.IsSuccessStatusCode, Is.True,
                $"Second player failed to join: {await joinResponse.Content.ReadAsStringAsync()}");

            // Spelet startar automatiskt när spelare 2 jointar (SetSessionActiveAsync).
            // Navigera bort och tillbaka så att Game.razor laddar om med status InProgress
            // och visar ActiveGame-komponenten istället för lobby-vyn
            await _page.EvaluateAsync("Blazor.navigateTo('/game', false, false)");
            await Task.Delay(500);
            await _page.EvaluateAsync("(url) => Blazor.navigateTo(url, false, false)", $"/game/{_gameCode}");
            await Task.Delay(2000);
        }

        [Then("I should be on the active game page")]
        public async Task ThenIShouldBeOnTheActiveGamePage()
        {
            await _page.WaitForSelectorAsync("text=Game in progress", new PageWaitForSelectorOptions { Timeout = 10000 });
        }

        [Then("I should see the current word")]
        public async Task ThenIShouldSeeTheCurrentWord()
        {
            await _page.WaitForSelectorAsync("text=Current word");
        }

        [Then("I should see the required letter")]
        public async Task ThenIShouldSeeTheRequiredLetter()
        {
            await _page.WaitForSelectorAsync("text=Next word must start with");
        }

        [Then("I should see the scoreboard")]
        public async Task ThenIShouldSeeTheScoreboard()
        {
            await _page.WaitForSelectorAsync(".list-group");
            var items = await _page.QuerySelectorAllAsync(".list-group-item");
            Assert.That(items, Has.Count.GreaterThanOrEqualTo(1));
        }
    }
}
