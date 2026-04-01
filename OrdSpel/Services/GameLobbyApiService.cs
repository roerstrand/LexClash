using System.Net;
using System.Net.Http.Headers;
using OrdSpel.Shared.DTOs;

namespace OrdSpel.UI.Services
{
    public class GameLobbyApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthStateService _authState;

        public GameLobbyApiService(HttpClient httpClient, AuthStateService authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }

        private void SetAuthHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }

        public async Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return null;
            }

            SetAuthHeader();

            // Call the API to get the lobby status for the specified game code
            // Uri.EscapeDataString(gameCode) for encoding the game code in case it contains special characters
            using var response = await _httpClient.GetAsync($"api/game/{Uri.EscapeDataString(gameCode)}/lobby", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameLobbyStatusDto>(ct);
        }
    }
}
