using OrdSpel.Shared.UserDtos;
using OrdSpel.UI.Models;

namespace OrdSpel.UI.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void LoginUser(string username, string password)
        {
            
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
