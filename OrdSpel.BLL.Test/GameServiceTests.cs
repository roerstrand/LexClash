using Moq;
using OrdSpel.BLL.Interfaces;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.Enums;
using OrdSpel.Shared.GameDTOs;
using Xunit;

namespace OrdSpel.BLL.Test
{
    public class GameServiceTests
    {
        private (GameService service, Mock<IGameRepository> mockRepo) CreateService()
        {
            var mockRepo = new Mock<IGameRepository>();
            var mockUserNameResolver = new Mock<IUserNameResolver>();
            mockUserNameResolver
                .Setup(r => r.GetUsernamesAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new Dictionary<string, string>());
            var service = new GameService(mockRepo.Object, mockUserNameResolver.Object);
            return (service, mockRepo);
        }

        private GameSessionResponseDto MakeSession(string code, GameStatus status, List<string>? playerIds = null)
        {
            return new GameSessionResponseDto
            {
                GameCode = code,
                Status = status,
                CategoryId = 1,
                StartWord = "hund",
                PlayerIds = playerIds ?? new List<string>()
            };
        }


        // ─── CreateGameAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task CreateGameAsync_ReturnsDto_WhenValidInput()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var dto = new CreateGameDto { CategoryId = 1 };
            var userId = "user1";
            var expectedSession = MakeSession("123456", GameStatus.WaitingForPlayers, new List<string> { userId });

            mockRepo.Setup(r => r.CodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            mockRepo.Setup(r => r.GetWordsByCategoryAsync(1)).ReturnsAsync(new List<string> { "hund", "katt" });
            mockRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), 1, It.IsAny<string>(), userId))
                    .ReturnsAsync(expectedSession);

            // Act
            var result = await service.CreateGameAsync(dto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123456", result.GameCode);
            Assert.Equal(GameStatus.WaitingForPlayers, result.Status);
            Assert.Contains(userId, result.PlayerIds);
        }

        [Fact]
        public async Task CreateGameAsync_RetriesCode_WhenCodeAlreadyExists()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var dto = new CreateGameDto { CategoryId = 1 };
            var callCount = 0;

            mockRepo.Setup(r => r.CodeExistsAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => callCount++ == 0);

            mockRepo.Setup(r => r.GetWordsByCategoryAsync(1)).ReturnsAsync(new List<string> { "hund" });
            mockRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), 1, It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(MakeSession("654321", GameStatus.WaitingForPlayers));

            // Act
            await service.CreateGameAsync(dto, "user1");

            // Assert
            mockRepo.Verify(r => r.CodeExistsAsync(It.IsAny<string>()), Times.AtLeast(2));
        }


        // ─── JoinGameAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task JoinGameAsync_ReturnsFail_WhenSessionNotFound()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            mockRepo.Setup(r => r.GetSessionByCodeAsync("000000")).ReturnsAsync((GameSessionResponseDto?)null);

            // Act
            var result = await service.JoinGameAsync(new JoinGameDto { GameCode = "000000" }, "user2");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Game not found.", result.Error);
        }

        [Fact]
        public async Task JoinGameAsync_ReturnsFail_WhenUserAlreadyInGame()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var session = MakeSession("123456", GameStatus.WaitingForPlayers, new List<string> { "user1" });
            mockRepo.Setup(r => r.GetSessionByCodeAsync("123456")).ReturnsAsync(session);

            // Act
            var result = await service.JoinGameAsync(new JoinGameDto { GameCode = "123456" }, "user1");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("You are already in this game.", result.Error);
        }

        [Fact]
        public async Task JoinGameAsync_ReturnsFail_WhenGameIsFull()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var session = MakeSession("123456", GameStatus.WaitingForPlayers, new List<string> { "user1", "user2" });
            mockRepo.Setup(r => r.GetSessionByCodeAsync("123456")).ReturnsAsync(session);

            // Act
            var result = await service.JoinGameAsync(new JoinGameDto { GameCode = "123456" }, "user3");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Game is full.", result.Error);
        }

        [Fact]
        public async Task JoinGameAsync_ReturnsFail_WhenGameAlreadyStarted()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var session = MakeSession("123456", GameStatus.InProgress, new List<string> { "user1" });
            mockRepo.Setup(r => r.GetSessionByCodeAsync("123456")).ReturnsAsync(session);

            // Act
            var result = await service.JoinGameAsync(new JoinGameDto { GameCode = "123456" }, "user2");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Game has already started.", result.Error);
        }

        [Fact]
        public async Task JoinGameAsync_ReturnsSuccess_WhenValidJoin()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var sessionBefore = MakeSession("123456", GameStatus.WaitingForPlayers, new List<string> { "user1" });
            var sessionAfter = MakeSession("123456", GameStatus.InProgress, new List<string> { "user1", "user2" });

            mockRepo.SetupSequence(r => r.GetSessionByCodeAsync("123456"))
                    .ReturnsAsync(sessionBefore)
                    .ReturnsAsync(sessionAfter);

            mockRepo.Setup(r => r.AddPlayerAsync("123456", "user2", 2)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SetSessionActiveAsync("123456")).Returns(Task.CompletedTask);

            // Act
            var result = await service.JoinGameAsync(new JoinGameDto { GameCode = "123456" }, "user2");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(GameStatus.InProgress, result.Data!.Status);
            Assert.Contains("user2", result.Data.PlayerIds);
        }


        // ─── GetGameAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetGameAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            mockRepo.Setup(r => r.GetSessionByCodeAsync("000000")).ReturnsAsync((GameSessionResponseDto?)null);

            // Act
            var result = await service.GetGameAsync("000000");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGameAsync_ReturnsDto_WhenFound()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var session = MakeSession("123456", GameStatus.WaitingForPlayers);
            mockRepo.Setup(r => r.GetSessionByCodeAsync("123456")).ReturnsAsync(session);

            // Act
            var result = await service.GetGameAsync("123456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123456", result!.GameCode);
        }
    }
}
