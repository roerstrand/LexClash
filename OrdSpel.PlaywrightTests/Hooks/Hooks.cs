using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace OrdSpel.PlaywrightTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        public IPage Page { get; private set; }

        [BeforeScenario]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false // sätt till true för att köra utan webbläsare
            });
            Page = await _browser.NewPageAsync();
        }

        [AfterScenario]
        public async Task Teardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
