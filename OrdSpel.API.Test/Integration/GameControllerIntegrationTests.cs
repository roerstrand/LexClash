using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using OrdSpel.API;
using OrdSpel.Shared.GameDTOs;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.Enums;

// Integration tests for GameController using WebApplicationFactory
// Important notes (read before editing tests):
// - Tests run against an in-memory Test environment configured in Program.cs when env = "Test"
// - Database seeded in Program.cs (SeededUserData, SeededAppData) will run; tests can rely on seeded categories/users
// - These tests use cookie-based authentication configured by Identity. For simplicity we call public endpoints only
// - Keep tests deterministic: do not rely on external services or network

namespace OrdSpel.API.Test.Integration
{
    public class GameControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GameControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // set environment to Test to use in-memory DB
                builder.UseSetting("environment", "Test");
            });
        }

        [Fact]
        public async Task GetLobbyStatus_Returns_Unauthorized_When_NotAuthenticated()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/game/randomcode/lobby");

            // Assert
            // GameController is protected with [Authorize] so unauthenticated requests return 401
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task GetGameStatus_Returns_Unauthorized_When_NotAuthenticated()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/game/randomcode/status");

            // Assert
            // GameController is protected with [Authorize] so unauthenticated requests return 401
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task CreateGame_RequiresAuthentication()
        {
            // Arrange
            var client = _factory.CreateClient();
            var dto = new CreateGameDto { CategoryId = 1 };

            // Act
            var resp = await client.PostAsJsonAsync("/api/game/create", dto);

            // Assert - should be 401 because no auth cookie
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
    }
}
