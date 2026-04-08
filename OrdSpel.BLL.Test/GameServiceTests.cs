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
        // Hjälpmetod för att skapa en GameService med ett mockat repository
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

        // Hjälpmetod för att skapa en enkel GameSessionResponseDto
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

        // Test 1: Skapar spel och returnerar korrekt DTO
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

        // Test 2: Genererar ny spelkod om koden redan finns
        [Fact]
        public async Task CreateGameAsync_RetriesCode_WhenCodeAlreadyExists()
        {
            // Arrange
            var (service, mockRepo) = CreateService();
            var dto = new CreateGameDto { CategoryId = 1 };
            var callCount = 0;

            // Första anropet returnerar true (kod finns), andra false (unik kod)
            mockRepo.Setup(r => r.CodeExistsAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => callCount++ == 0);

            mockRepo.Setup(r => r.GetWordsByCategoryAsync(1)).ReturnsAsync(new List<string> { "hund" });
            mockRepo.Setup(r => r.CreateSessionAsync(It.IsAny<string>(), 1, It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(MakeSession("654321", GameStatus.WaitingForPlayers));

            // Act
            await service.CreateGameAsync(dto, "user1");

            // Assert – CodeExistsAsync ska ha anropats minst 2 gånger
            mockRepo.Verify(r => r.CodeExistsAsync(It.IsAny<string>()), Times.AtLeast(2));
        }


        // ─── JoinGameAsync ─────────────────────────────────────────────────────

        // Test 3: Returnerar fel om spelet inte hittas
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
            Assert.Equal("Spelet hittades inte.", result.Error);
        }

        // Test 4: Returnerar fel om användaren redan är med i spelet
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
            Assert.Equal("Du är redan med i det här spelet.", result.Error);
        }

        // Test 5: Returnerar fel om spelet är fullt (2 spelare)
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
            Assert.Equal("Spelet är fullt.", result.Error);
        }

        // Test 6: Returnerar fel om spelet redan har startat
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
            Assert.Equal("Spelet har redan startat.", result.Error);
        }

        // Test 7: Lyckat join – uppdaterad session returneras
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

        // Test 8: Returnerar null om spelet inte hittas
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

        // Test 9: Returnerar korrekt DTO om spelet hittas
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
