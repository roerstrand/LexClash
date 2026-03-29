using OrdSpel.DAL.Repositories;
using OrdSpel.Shared;
using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly Random _random = new();

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GameSessionResponseDto> CreateGameAsync(CreateGameDto dto, string userId)
        {
            var code = await GenerateUniqueCodeAsync();

            var words = await _gameRepository.GetWordsByCategoryAsync(dto.CategoryId);
            var startWord = words[_random.Next(words.Count)];

            return await _gameRepository.CreateSessionAsync(code, dto.CategoryId, startWord, userId);
        }

        public async Task<GameSessionResponseDto?> JoinGameAsync(JoinGameDto dto, string userId)
        {
            var session = await _gameRepository.GetSessionByCodeAsync(dto.GameCode);

            if (session == null) return null;
            if (session.Status != GameStatus.Waiting) return null;
            if (session.PlayerIds.Contains(userId)) return null;
            if (session.PlayerIds.Count >= 2) return null;

            await _gameRepository.AddPlayerAsync(dto.GameCode, userId, 2);
            await _gameRepository.SetSessionActiveAsync(dto.GameCode);

            return await _gameRepository.GetSessionByCodeAsync(dto.GameCode);
        }

        public async Task<GameSessionResponseDto?> GetGameAsync(string gameCode)
        {
            return await _gameRepository.GetSessionByCodeAsync(gameCode);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            do
            {
                code = _random.Next(100000, 999999).ToString();
            }
            while (await _gameRepository.CodeExistsAsync(code));

            return code;
        }
    }
}
