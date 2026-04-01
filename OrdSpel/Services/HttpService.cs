using System.Net.Http.Headers;
using OrdSpel.Shared.AuthDTOs;
using OrdSpel.UI.Interfaces;

namespace OrdSpel.UI.Services
{
    public class HttpService : IHttpService
    {
        public HttpClient _httpClient { get; }

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }


        public async Task<AuthResult> LoginUser(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {

                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return new AuthResult { Success = true, Token = result?.Token };
            }

            var errorMessage = await response.Content.ReadAsStringAsync();

            return new AuthResult { Success = false, ErrorMessage = errorMessage };
        }

        public async Task<AuthResult> RegisterUser(RegisterDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return new AuthResult { Success = true, Token = result?.Token }; //Returnerar AuthResult-modell (modell i UI:t!)
            }

            var errorMessage = await response.Content.ReadAsStringAsync();

            return new AuthResult { Success = false, ErrorMessage = errorMessage };
        }

    }

}
