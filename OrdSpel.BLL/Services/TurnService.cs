using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared;
using OrdSpel.Shared.Constraints;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Services
{
    public class TurnService : ITurnService
    {
        private readonly ITurnRepository _turnRepository;

        public TurnService(ITurnRepository turnRepository)
        {
            _turnRepository = turnRepository;
        }
        public async Task<(TurnResponseDto? response, string? error)> PlayTurnAsync(string gameCode, string userId, TurnRequestDto dto)
        {
            var session = await _turnRepository.GetSessionWithDetailsAsync(gameCode);

            if (session == null)
            {
                return (null, "Game not found");
            }

            if (session.Status != GameStatus.InProgress)
            {
                return (null, "Game is not active");
            }

            if (session.CurrentTurnUserId != userId)
            {
                return (null, "It is not your turn");
            }

            int score;

            if (dto.PassedTurn)
            {
                score = GameRules.PassPenalty;

                var usedWords = session.Turns.Where(t => t.Word != null).Select(t => t.Word!).ToHashSet();
                var randomWord = await _turnRepository.GetRandomWordAsync(session.CategoryId, usedWords);

                if (randomWord != null)
                {
                    dto.Word = randomWord.Text;
                }
                else
                {
                    var lastTurn = session.Turns.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
                    dto.Word = lastTurn?.Word ?? session.StartWord;
                }
            }
            else
            {
                var word = dto.Word?.Trim().ToLower();

                if (string.IsNullOrEmpty(word))
                {
                    return (null, "No word provided");
                }

                var lastWord = session.Turns.Any()
                             ? session.Turns.OrderByDescending(t => t.CreatedAt).First().Word
                             ?? session.StartWord
                             : session.StartWord;

                if (word[0] != lastWord[^1])
                {
                    return (null, $"The word must start with the last letter of the previous word '{lastWord[^1]}'.");
                }

                var wordInCategory = await _turnRepository.GetWordAsync(word, session.CategoryId);

                if (wordInCategory == null)
                {
                    return (null, "The word is not in the selected category");
                }

                var usedWords = session.Turns.Select(t => t.Word).ToHashSet();

                if (usedWords.Contains(word))
                {
                    return (null, "The word has already been used in this game.");
                }

                score = word.Sum(c => LetterScores.GetScore(c));

                if (word.Length >= GameRules.LongWordThreshold)
                {
                    score += GameRules.LongWordBonus;
                }

                if (wordInCategory.IsHard)
                {
                    score += GameRules.HardWordBonus;
                }

                dto.Word = word;
            }

            var turn = new GameTurn
            {
                SessionId = session.Id,
                Round = session.CurrentRound,
                UserId = userId,
                Word = dto.Word,
                Score = score,
                PassedTurn = dto.PassedTurn,
                CreatedAt = DateTime.UtcNow
            };

            await _turnRepository.AddTurnAsync(turn);

            var player = session.Players.First(p => p.UserId == userId);
            player.TotalScore += score;

            var nextPlayer = session.Players.First(p => p.UserId != userId);
            session.CurrentTurnUserId = nextPlayer.UserId;
            session.CurrentRound++;

            bool gameIsFinished = session.CurrentRound > GameRules.TotalRounds;

            if(gameIsFinished)
            {
                session.Status = GameStatus.GameFinished;
            }

            await _turnRepository.SaveChangesAsync();

            return (new TurnResponseDto
            {
                Score = score,
                TotalScore = player.TotalScore,
                NextUserId = nextPlayer.UserId,
                CurrentRound = session.CurrentRound,
                GameFinished = gameIsFinished
            }, null);
        }
    }
}
