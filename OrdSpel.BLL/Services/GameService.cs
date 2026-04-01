using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared;
using OrdSpel.Shared.Enums;
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

        public async Task<ServiceResult<GameSessionResponseDto>> JoinGameAsync(JoinGameDto dto, string userId)
        {
            var session = await _gameRepository.GetSessionByCodeAsync(dto.GameCode);

            if (session == null)
                return ServiceResult<GameSessionResponseDto>.Fail("Spelet hittades inte.");
            if (session.PlayerIds.Contains(userId))
                return ServiceResult<GameSessionResponseDto>.Fail("Du är redan med i det här spelet.");
            if (session.PlayerIds.Count >= 2)
                return ServiceResult<GameSessionResponseDto>.Fail("Spelet är fullt.");
            if (session.Status != GameStatus.WaitingForPlayers)
                return ServiceResult<GameSessionResponseDto>.Fail("Spelet har redan startat.");

            await _gameRepository.AddPlayerAsync(dto.GameCode, userId, 2);
            await _gameRepository.SetSessionActiveAsync(dto.GameCode);

            var updated = await _gameRepository.GetSessionByCodeAsync(dto.GameCode);
            return ServiceResult<GameSessionResponseDto>.Ok(updated!);
        }

        public async Task<ServiceResult<GameSessionResponseDto>> EndGameAsync(string gameCode, string userId)
        {
            var session = await _gameRepository.GetSessionByCodeAsync(gameCode);

            if (session == null)
                return ServiceResult<GameSessionResponseDto>.Fail("Spelet hittades inte.");
            if (!session.PlayerIds.Contains(userId))
                return ServiceResult<GameSessionResponseDto>.Fail("Du är inte med i det här spelet.");
            if (session.Status == GameStatus.GameFinished)
                return ServiceResult<GameSessionResponseDto>.Fail("Spelet är redan avslutat.");

            await _gameRepository.SetSessionFinishedAsync(gameCode);

            var updated = await _gameRepository.GetSessionByCodeAsync(gameCode);
            return ServiceResult<GameSessionResponseDto>.Ok(updated!);
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
