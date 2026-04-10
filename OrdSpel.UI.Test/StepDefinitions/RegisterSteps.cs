using Microsoft.Playwright;
using Reqnroll;
using System.Text;
using System.Text.Json;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class RegisterSteps
    {
        private readonly IPage _page;
        private readonly string _apiBaseUrl = "https://localhost:7099";

        public RegisterSteps(Hooks.Hooks hooks)
        {
            _page = hooks.Page;
        }

        [AfterScenario]
        public async Task Cleanup()
        {
            try
            {
                // API:t använder cookie-auth, inte JWT – samma HttpClient-instans bevarar cookien
                var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
                using var client = new HttpClient(handler);

                var loginBody = JsonSerializer.Serialize(new { username = "testuser", password = "Test123!" });
                var loginResponse = await client.PostAsync($"{_apiBaseUrl}/api/auth/login",
                    new StringContent(loginBody, Encoding.UTF8, "application/json"));

                if (!loginResponse.IsSuccessStatusCode) return;

                // Auth-cookien sätts automatiskt av HttpClientHandler, skickas med i delete-anropet
                await client.DeleteAsync($"{_apiBaseUrl}/api/auth/delete");
            }
            catch { /* testanvändaren finns inte, inget att städa */ }
        }

        [Given("I am on the register page")]
        public async Task GivenIAmOnTheRegisterPage()
        {
            await _page.GotoAsync("https://localhost:7265/register");
        }

        [When("I fill in username {string} and password {string} and confirm the password")]
        public async Task WhenIFillInUsernameAndPassword(string username, string password)
        {
            await _page.FillAsync("input:not([type='password']):not([type='hidden'])", username);
            await _page.Locator("input[type='password']").Nth(0).FillAsync(password);
            await _page.Locator("input[type='password']").Nth(1).FillAsync(password);
            await _page.ClickAsync("button[type='submit']");
        }

        [Then("I should be redirected to the login page")]
        public async Task ThenIShouldBeRedirectedToTheLoginPage()
        {
            // Register.razor kör Navigation.NavigateTo("/") = klient-side-navigation till startsidan
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex(@"localhost:7265/$"),
                new PageWaitForURLOptions { Timeout = 10000 });
        }
    }
}
