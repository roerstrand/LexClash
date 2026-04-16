using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;
using OrdSpel.Shared.Enums;
using OrdSpel.UI.Interfaces;

namespace OrdSpel.UI.Services
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthStateService _authState;

        public GameService(HttpClient httpClient, AuthStateService authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }

        private void SetAuthHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            if (_authState.CookieValue != null)
                _httpClient.DefaultRequestHeaders.Add("Cookie", _authState.CookieValue);
        }

        public async Task<List<T>> GetCategoriesAsync<T>()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("api/category");

            if (!response.IsSuccessStatusCode)
                return new List<T>();

            return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
        }

        public async Task<GameSessionResponseDto?> CreateGameAsync(CreateGameDto dto)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/game/create", dto);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameSessionResponseDto>();
        }

        public async Task<(GameSessionResponseDto? Result, string? Error)> JoinGameAsync(JoinGameDto dto)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/game/join", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameSessionResponseDto>();
                return (result, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (null, error);
        }

        public async Task<GameSessionResponseDto?> GetGameAsync(string gameCode)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"api/game/{gameCode}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameSessionResponseDto>();
        }

        public async Task<GameSessionResponseDto?> GetActiveGameAsync()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("api/game/active");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameSessionResponseDto>();
        }

        public async Task<GameStatusDto?> GetGameStatusAsync(string gameCode)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"api/game/{gameCode}/status");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameStatusDto>();
        }

        public async Task EndGameAsync(string gameCode)
        {
            SetAuthHeader();
            await _httpClient.PutAsync($"api/game/end/{gameCode}", null);
        }

        public async Task<GameResultDto?> GetGameResultAsync(string gameCode)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"api/game/{gameCode}/result");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameResultDto>();
        }

        public async Task<bool> DeleteGameAsync(string gameCode)
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/game/{gameCode}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<GameSummaryDto>> GetGameHistoryAsync()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("api/game/history");

            if (!response.IsSuccessStatusCode)
                return new List<GameSummaryDto>();

            return await response.Content.ReadFromJsonAsync<List<GameSummaryDto>>() ?? new List<GameSummaryDto>();
        }

        public async Task<(TurnResponseDto? Result, string? Error)> SubmitTurnAsync(string gameCode, TurnRequestDto dto)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"api/games/{gameCode}/turns", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TurnResponseDto>();
                return (result, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (null, error);
        }
    }
}
