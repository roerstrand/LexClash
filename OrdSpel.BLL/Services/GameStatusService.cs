using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Services
{
    public class GameStatusService : IGameStatusService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IUserNameResolver _userNameResolver;

        public GameStatusService(IGameSessionRepository gameSessionRepository, IUserNameResolver userNameResolver)
        {
            _gameSessionRepository = gameSessionRepository;
            _userNameResolver = userNameResolver;
        }

        public async Task<GameStatusDto?> GetGameStatusAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
                return null;

            var session = await _gameSessionRepository.GetByGameCodeWithDetailsAsync(gameCode);
            if (session == null)
                return null;

            var lastTurn = session.Turns
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            return new GameStatusDto
            {
                SessionId = session.Id,
                GameCode = session.GameCode,
                Status = session.Status,
                CurrentRound = session.CurrentRound,
                CurrentTurnUserId = session.CurrentTurnUserId,
                StartWord = session.StartWord,
                LastWord = lastTurn?.Word,
                LastTurnWasPass = lastTurn?.PassedTurn ?? false,
                Players = await MapPlayersWithUsernamesAsync(session.Players)
            };
        }

        private async Task<List<GamePlayerStatusDto>> MapPlayersWithUsernamesAsync(IEnumerable<DAL.Models.GamePlayer> players)
        {
            var userIds = players.Select(p => p.UserId);
            var usernames = await _userNameResolver.GetUsernamesAsync(userIds);

            return players.Select(p => new GamePlayerStatusDto(
                p.UserId,
                usernames.GetValueOrDefault(p.UserId),
                p.PlayerOrder,
                p.TotalScore
            )).ToList();
        }
    }
}
