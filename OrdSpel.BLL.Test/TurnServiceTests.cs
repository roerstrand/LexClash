using Moq;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.Constraints;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xunit;

namespace OrdSpel.BLL.Test
{
    public class TurnServiceTests
    {
        private readonly Mock<ITurnRepository> _repoMock;
        private readonly TurnService _turnService;

        public TurnServiceTests()
        {
            _repoMock = new Mock<ITurnRepository>();
            _turnService = new TurnService(_repoMock.Object);
        }

        [Fact]
        public async Task PlayTurnAsync_SessionSuccess()
        {
            var session = new GameSession
            {
                Id = 1,
                GameCode = "ABC",
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user1",
                CurrentRound = 1,
                CategoryId = 1,
                StartWord = "katt",
                Players = new List<GamePlayer>
                {
                    new GamePlayer { UserId = "user1", TotalScore = 0 },
                    new GamePlayer { UserId = "user2", TotalScore = 0 }
                },
                Turns = new List<GameTurn>()
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("ABC"))
                .ReturnsAsync(session);

            _repoMock.Setup(r => r.GetWordAsync("tiger", 1))
                .ReturnsAsync(new Word
                {
                    Text = "tiger",
                    CategoryId = 1,
                    IsHard = false
                });

            var dto = new TurnRequestDto
            {
                Word = "tiger",
                PassedTurn = false
            };

            var (response, error) = await _turnService.PlayTurnAsync("ABC", "user1", dto);

            Assert.Null(error);
            Assert.NotNull(response);
            Assert.Equal("user2", response.NextUserId);
            Assert.Equal(2, response.CurrentRound);
        }

        [Fact]
        public async Task PlayTurnAsync_SessionFail()
        {
            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
                .ReturnsAsync((GameSession?)null);

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", new TurnRequestDto());

            Assert.Null(response);
            Assert.Equal("Game not found", error);
        }

        [Fact]
        public async Task PlayTurnAsync_WrongPlayer()
        {
            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user2"
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
                .ReturnsAsync(session);

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", new TurnRequestDto());

            Assert.Null(response);
            Assert.NotNull(error);
            Assert.Equal("It is not your turn", error);
        }

        [Fact]
        public async Task PlayTurnAsync_PassedTurn()
        {
            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user1",
                Players = new List<GamePlayer>
                {
                    new GamePlayer { UserId = "user1", TotalScore = 0 },
                    new GamePlayer { UserId = "user2", TotalScore = 0 }
                },
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
               .ReturnsAsync(session);

            var dto = new TurnRequestDto
            {
                PassedTurn = true
            };

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            Assert.Null(error);
            Assert.NotNull(response);
            Assert.Equal(GameRules.PassPenalty, response.Score);
            Assert.Equal("user2", response.NextUserId);
            Assert.NotEqual("user1", response.NextUserId);
        }

        [Fact]
        public async Task PlayTurnAsync_WordNotstartingWithCorrectLetter()
        {
            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user1",
                StartWord = "hund",
                Turns = new List<GameTurn>
                {
                    new GameTurn
                    {
                        Word = "delfin",
                        CreatedAt = DateTime.UtcNow
                    }
                },
                Players = new List<GamePlayer>
                {
                    new GamePlayer { UserId = "user1" }
                }
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
                .ReturnsAsync(session);

            var dto = new TurnRequestDto
            {
                Word = "tiger"
            };

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            Assert.NotNull(error);
            Assert.Null(response);
            Assert.Equal($"The word must start with the last letter of the previous word 'n'.", error);
        }

        [Fact]
        public async Task PlayTurnAsync_LongWordBonus()
        {
            var longWord = "nile crocodile";

            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user1",
                StartWord = "salmon",
                Turns = new List<GameTurn>(),
                Players = new List<GamePlayer>
            {
                new GamePlayer { UserId = "user1" },
                new GamePlayer { UserId = "user2" }
            }
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123")).ReturnsAsync(session);
            _repoMock.Setup(r => r.GetWordAsync(longWord, It.IsAny<int>()))
                     .ReturnsAsync(new Word { Text = longWord, IsHard = false });

            var dto = new TurnRequestDto
            {
                Word = longWord
            };

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            var expectedWordScore = longWord.Sum(c => LetterScores.GetScore(c));

            Assert.Equal(expectedWordScore + GameRules.LongWordBonus, response?.Score);
        }
    }
}
