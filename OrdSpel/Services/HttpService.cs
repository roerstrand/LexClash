using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.UI.Services
{
    public class HttpService
    {
        public HttpClient _httpClient { get; }

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<ActionResult> LoginUser(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (!response.IsSuccessStatusCode)
                return new UnauthorizedResult();

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result!.Token);

            return new OkResult();
        }

        public async Task<AuthResult> RegisterUser(RegisterDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return new AuthResult { Success = true, Token = result?.Token }; //Returnerar AuthResult-modell (medell i UI:t!)
            }

            return new AuthResult { Success = false, ErrorMessage = "Något gick fel." };
        }

    }

}
