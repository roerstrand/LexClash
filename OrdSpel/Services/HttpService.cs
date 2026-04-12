using System.Text.Json;
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

        private async Task StoreAuthCookieAsync(HttpResponseMessage response)
        {
            string? userId = null;
            string? username = null;

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("userId", out var idProp))
                    userId = idProp.GetString();
                if (root.TryGetProperty("username", out var nameProp))
                    username = nameProp.GetString();
            }
            catch { }

            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var authCookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Identity.Application="));
                if (authCookie != null)
                    _authState.Login(authCookie.Split(';')[0], userId, username);
            }
        }

        public async Task<AuthResult> LoginUser(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                await StoreAuthCookieAsync(response);
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
                await StoreAuthCookieAsync(response);
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
