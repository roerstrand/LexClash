using Microsoft.EntityFrameworkCore;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.Shared;
using Xunit;

namespace OrdSpel.BLL.Test
{
    public class GameLobbyServiceTests
    {
        // Use EF Core InMemory Database to simulate an in‑memory database.
        // Each test creates its own fresh database instance, keeping tests isolated so they don’t interfere with one another.
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }



        // Test 1: returns null when the game code does not exist in the database.
        [Fact]
        public async Task GetLobbyStatusAsync_ReturnsNull_WhenGameCodeDoesNotExist()
        {
            // Arrange
            var context = CreateContext();
            var service = new GameLobbyService(context);

            // Act
            var result = await service.GetLobbyStatusAsync("NOPE");

            // Assert
            Assert.Null(result);
        }



        // Test 2: If the session and all required data are present, return the correct DTO.
        [Fact]
        public async Task GetLobbyStatusAsync_ReturnsLobbyStatus_WhenSessionExists()
        {
            // Arrange
            var context = CreateContext();

            // Create a category "Djur"
            var category = new Category
            {
                Name = "Djur"
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();


            // Create a game session associating with the category and a start word "hund"
            var session = new GameSession
            {
                GameCode = "ABC123",
                Status = GameStatus.WaitingForPlayers,
                CategoryId = category.Id,
                Category = category,
                StartWord = "hund",
                CurrentRound = 1,
                CreatedAt = DateTime.UtcNow
            };

            context.GameSessions.Add(session);
            await context.SaveChangesAsync();

            // Add one player to the session
            context.GamePlayers.Add(new GamePlayer
            {
                SessionId = session.Id,
                UserId = "user1",
                PlayerOrder = 1,
                TotalScore = 0
            });

            await context.SaveChangesAsync();

            // Call GetLobbyStatusAsyn and verify the returned DTO contains correct data based on the session and player we created.
            var service = new GameLobbyService(context);



            // Act
            var result = await service.GetLobbyStatusAsync("ABC123");


            // Assert
            Assert.NotNull(result);
            Assert.Equal("ABC123", result!.GameCode);
            Assert.Equal("Djur", result.CategoryName);
            Assert.Equal("hund", result.StartWord);
            Assert.Equal(GameStatus.WaitingForPlayers, result.Status);
            Assert.Equal(1, result.PlayerCount);
            Assert.False(result.IsReadyToStart);
        }




        // Test 3: 2 players + status is InProgress => IsReadyToStart should be true
        [Fact]
        public async Task GetLobbyStatusAsync_IsReadyToStart_True_WhenTwoPlayersAndInProgress()
        {
            // Arrange: Create a simulated database context 
            var context = CreateContext();

            // create a category (for GameSession can refer to CategoryId)
            var category = new Category
            {
                Name = "Djur"
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Create a game session (Status.Inprogress && startWord = "kat")
            var session = new GameSession
            {
                GameCode = "XYZ789",
                Status = GameStatus.InProgress,
                CategoryId = category.Id,
                Category = category,
                StartWord = "katt",
                CurrentRound = 1,
                CurrentTurnUserId = "user1",
                CreatedAt = DateTime.UtcNow
            };

            context.GameSessions.Add(session);
            await context.SaveChangesAsync();

            // Add 2 players to the session
            context.GamePlayers.AddRange(
                new GamePlayer
                {
                    SessionId = session.Id,
                    UserId = "user1",
                    PlayerOrder = 1,
                    TotalScore = 0
                },
                new GamePlayer
                {
                    SessionId = session.Id,
                    UserId = "user2",
                    PlayerOrder = 2,
                    TotalScore = 0
                });

            await context.SaveChangesAsync();

            // Call Service to test
            var service = new GameLobbyService(context);

            // Act: Call  to retrieve the lobby status for this game code
            var result = await service.GetLobbyStatusAsync("XYZ789");

            Assert.NotNull(result);
            Assert.Equal(2, result!.PlayerCount);
            Assert.True(result.IsReadyToStart);
            Assert.Equal(GameStatus.InProgress, result.Status);
            Assert.Equal("user1", result.CurrentTurnUserId);
        }
    }
}