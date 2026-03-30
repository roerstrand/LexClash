using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Reqnroll;
//using TechTalk.SpecFlow;

namespace OrdSpel.PlaywrightTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private IPlaywright _playwright;
        private IBrowser _browser;

        public string BaseUrl { get; set; }
        public IPage Page { get; private set; }

        [BeforeScenario]
        public async Task Setup()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            BaseUrl = config["BaseUrl"];

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new () {
                
                    Headless = false
                 // sätt till true för att köra utan webbläsare
            });

            var context = await _browser.NewContextAsync();
            Page = await context.NewPageAsync();
        }

        [AfterScenario]
        public async Task Teardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
