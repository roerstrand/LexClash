using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.UI.Services
{
    public class HttpService
    {
        public HttpClient HttpClient { get; }


        public async Task<ActionResult> LoginUser(LoginDto loginDto)
        {
            var response = await HttpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (!response.IsSuccessStatusCode)
                return new UnauthorizedResult();

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);


        }

        public void RegisterUser(string username, string password, string email)
        {
            // Implement registration logic here
        }
    }
}
