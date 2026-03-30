using OrdSpel.Shared.GameDTOs;
using System.Net.Http.Headers;

namespace OrdSpel.UI.Services
{
    public class GameService
    {
        private readonly HttpClient _httpClient;
        private readonly AppState _appState;

        public GameService(HttpClient httpClient, AppState appState)
        {
            _httpClient = httpClient;
            _appState = appState;
        }

        private void SetAuthHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _appState.Token);
        }

        // OBS: Stäm av  när kategori-feature är pushad — säkerställ att hela
        // flödet går via BLL-service och DAL-repository istället för direkt mot DbContext i controllern.
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
    }
}
