using OrdSpel.Shared.AuthDTOs;
using OrdSpel.UI.Interfaces;

namespace OrdSpel.UI.Services
{
    public class HttpService : IHttpService
    {
        public HttpClient _httpClient { get; }
        private readonly AuthStateService _authState;

        public HttpService(HttpClient httpClient, AuthStateService authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }

        // Läser Set-Cookie-headern från API-svaret och sparar cookievärdet i AuthStateService
        private void StoreAuthCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var authCookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Identity.Application="));
                if (authCookie != null)
                    _authState.Login(authCookie.Split(';')[0]); // Spara bara "name=value", inte path/expires/etc
            }
        }

        public async Task<AuthResult> LoginUser(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                StoreAuthCookie(response);
                return new AuthResult { Success = true };
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new AuthResult { Success = false, ErrorMessage = errorMessage };
        }

        public async Task<AuthResult> RegisterUser(RegisterDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                StoreAuthCookie(response);
                return new AuthResult { Success = true };
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new AuthResult { Success = false, ErrorMessage = errorMessage };
        }

        public async Task LogoutUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            if (_authState.CookieValue != null)
                request.Headers.Add("Cookie", _authState.CookieValue);

            await _httpClient.SendAsync(request);
            _authState.Logout();
        }
    }
}
