using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;

namespace OrdSpel.PlaywrightTests.Helpers
{
    // Använd denna klass i step definitions för att underlätta autentisering
    public class AuthHelper
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

        public AuthHelper(IPage page, string baseUrl)
        {
            _page = page;
            _baseUrl = baseUrl;
        }

        public async Task LoginAsync(string username, string password)
        {
            await _page.GotoAsync(_baseUrl);
            await _page.WaitForSelectorAsync("#username");
            await _page.FillAsync("#username", username);
            await _page.FillAsync("#password", password);
            await Task.WhenAll(
                _page.ClickAsync("#submitLogin"),
                _page.WaitForNavigationAsync()
            );
        }
    }
}
