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

        public GameStatusService(IGameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        public async Task<GameStatusDto?> GetGameStatusAsync(string gameCode)
        {
            //om gamecode inte finns, avbryt
            if (string.IsNullOrWhiteSpace(gameCode))
                return null;

            //hämta session, om den inte finns, avbryt
            var session = await _gameSessionRepository.GetByGameCodeWithDetailsAsync(gameCode);
            if (session == null)
                return null;

            //hämta senaste rundan
            var lastTurn = session.Turns
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            //mappa till GameStatusDto
            return new GameStatusDto
            {
                SessionId = session.Id,
                GameCode = session.GameCode,
                Status = session.Status,
                CurrentRound = session.CurrentRound,
                CurrentTurnUserId = session.CurrentTurnUserId,
                StartWord = session.StartWord,
                LastWord = lastTurn?.Word,
                Players = session.Players
                .Select(p => new GamePlayerStatusDto(p.UserId, p.PlayerOrder, p.TotalScore))
                .ToList()
            };
        }
    }
}
