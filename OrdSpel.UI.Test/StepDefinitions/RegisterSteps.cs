using Microsoft.Playwright;
using Reqnroll;
using System.Net.Http.Headers;
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
                var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
                using var client = new HttpClient(handler);

                var loginBody = JsonSerializer.Serialize(new { username = "testuser", password = "Test123!" });
                var loginResponse = await client.PostAsync($"{_apiBaseUrl}/api/auth/login",
                    new StringContent(loginBody, Encoding.UTF8, "application/json"));

                if (!loginResponse.IsSuccessStatusCode) return;

                var json = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
                var token = json.RootElement.GetProperty("token").GetString();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            await _page.WaitForURLAsync("https://localhost:7265/");
            Assert.That(_page.Url, Is.EqualTo("https://localhost:7265/"));
        }
    }
}
