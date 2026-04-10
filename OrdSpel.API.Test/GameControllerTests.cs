using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using OrdSpel.API.Controllers;
using OrdSpel.API.Hubs;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;
using OrdSpel.Shared.Enums;
using Xunit;

namespace OrdSpel.API.Test
{
    // Unit tests for GameController focusing on HTTP results and hub interaction
    public class GameControllerTests
    {
        private readonly Mock<IGameService> _gameServiceMock;
        private readonly Mock<IGameLobbyService> _gameLobbyServiceMock;
        private readonly Mock<IGameStatusService> _gameStatusServiceMock;
        private readonly Mock<IHubContext<GameHub>> _hubContextMock;
        private readonly GameController _controller;

        public GameControllerTests()
        {
            _gameServiceMock = new Mock<IGameService>();
            _gameLobbyServiceMock = new Mock<IGameLobbyService>();
            _gameStatusServiceMock = new Mock<IGameStatusService>();
            _hubContextMock = new Mock<IHubContext<GameHub>>();

            _controller = new GameController(
                _gameServiceMock.Object,
                _gameLobbyServiceMock.Object,
                _gameStatusServiceMock.Object,
                _hubContextMock.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            };

            var identity = new ClaimsIdentity(claims, "Test");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        // GetLobbyStatus: when service returns null -> NotFound
        [Fact]
        public async Task GetLobbyStatus_Returns_NotFound_WhenNull()
        {
            // Arrange
            _gameLobbyServiceMock.Setup(s => s.GetLobbyStatusAsync(It.IsAny<string>()))
                .ReturnsAsync((GameLobbyStatusDto?)null);

            // Act
            var actionResult = await _controller.GetLobbyStatus("abc");

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        // GetLobbyStatus: when service returns dto -> Ok with dto
        [Fact]
        public async Task GetLobbyStatus_Returns_Ok_WithDto()
        {
            // Arrange
            var dto = new GameLobbyStatusDto { GameCode = "abc" };
            _gameLobbyServiceMock.Setup(s => s.GetLobbyStatusAsync("abc")).ReturnsAsync(dto);

            // Act
            var actionResult = await _controller.GetLobbyStatus("abc");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(dto, ok.Value);
        }

        // JoinGame: when service fails -> BadRequest
        [Fact]
        public async Task JoinGame_Returns_BadRequest_When_ServiceFails()
        {
            // Arrange
            var dto = new JoinGameDto { GameCode = "abc" };
            _gameServiceMock.Setup(s => s.JoinGameAsync(It.IsAny<JoinGameDto>(), "user1"))
                .ReturnsAsync(ServiceResult<GameSessionResponseDto>.Fail("error"));

            // Act
            var result = await _controller.JoinGame(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // JoinGame: when ModelState invalid -> BadRequest
        [Fact]
        public async Task JoinGame_Returns_BadRequest_When_ModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("GameCode", "Required");

            // Act
            var result = await _controller.JoinGame(new JoinGameDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // JoinGame: unauthorized when user id missing -> Unauthorized
        [Fact]
        public async Task JoinGame_Returns_Unauthorized_When_NoUserId()
        {
            // Arrange - remove user claim to simulate unauthorized
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.JoinGame(new JoinGameDto { GameCode = "abc" });

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        // JoinGame: on success should call SignalR group send and return Ok
        [Fact]
        public async Task JoinGame_Returns_Ok_And_Invokes_Hub_OnSuccess()
        {
            // Arrange
            var dto = new JoinGameDto { GameCode = "abc" };
            var session = new GameSessionResponseDto { GameCode = "abc" };
            _gameServiceMock.Setup(s => s.JoinGameAsync(It.Is<JoinGameDto>(d => d.GameCode == "abc"), "user1"))
                .ReturnsAsync(ServiceResult<GameSessionResponseDto>.Ok(session));

            var clientsMock = new Mock<IHubClients>();
            var groupMock = new Mock<IClientProxy>();
            // ensure SendCoreAsync completes
            groupMock.Setup(g => g.SendCoreAsync("LobbyUpdated", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            clientsMock.Setup(c => c.Group("abc")).Returns(groupMock.Object);
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            // Act
            var result = await _controller.JoinGame(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            // Verify payload equals expected session
            Assert.Equal(session, ok.Value);
            groupMock.Verify(g => g.SendCoreAsync("LobbyUpdated", It.Is<object?[]>(a => a.Length == 1 && (string?)a[0] == "abc"), It.IsAny<CancellationToken>()), Times.Once);
        }

        // EndGame: when service fails -> BadRequest
        [Fact]
        public async Task EndGame_Returns_BadRequest_When_ServiceFails()
        {
            // Arrange
            _gameServiceMock.Setup(s => s.EndGameAsync("abc", "user1"))
                .ReturnsAsync(ServiceResult<GameSessionResponseDto>.Fail("not allowed"));

            // Act
            var result = await _controller.EndGame("abc");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // EndGame: when service succeeds -> Ok with session
        [Fact]
        public async Task EndGame_Returns_Ok_When_ServiceSucceeds()
        {
            // Arrange
            var session = new GameSessionResponseDto { GameCode = "abc" };
            _gameServiceMock.Setup(s => s.EndGameAsync("abc", "user1"))
                .ReturnsAsync(ServiceResult<GameSessionResponseDto>.Ok(session));

            // Act
            var result = await _controller.EndGame("abc");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(session, ok.Value);
        }

        // CreateGame: invalid model state -> BadRequest
        [Fact]
        public async Task CreateGame_Returns_BadRequest_When_ModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateGame(new CreateGameDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // CreateGame: unauthorized when user id missing -> Unauthorized
        [Fact]
        public async Task CreateGame_Returns_Unauthorized_When_NoUserId()
        {
            // Arrange - remove user claim to simulate unauthorized
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.CreateGame(new CreateGameDto());

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        // CreateGame: when service creates game -> Ok with result
        [Fact]
        public async Task CreateGame_Returns_Ok_When_Success()
        {
            // Arrange
            var createDto = new CreateGameDto { CategoryId = 1 };
            var created = new GameSessionResponseDto { GameCode = "xyz" };
            _gameServiceMock.Setup(s => s.CreateGameAsync(It.IsAny<CreateGameDto>(), "user1")).ReturnsAsync(created);

            // Act
            var result = await _controller.CreateGame(createDto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(created, ok.Value);
        }

        // GetGame: not found vs ok
        [Fact]
        public async Task GetGame_Returns_NotFound_When_Null()
        {
            // Arrange
            _gameServiceMock.Setup(s => s.GetGameAsync("abc")).ReturnsAsync((GameSessionResponseDto?)null);

            // Act
            var result = await _controller.GetGame("abc");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGame_Returns_Ok_When_Found()
        {
            // Arrange
            var session = new GameSessionResponseDto { GameCode = "abc" };
            _gameServiceMock.Setup(s => s.GetGameAsync("abc")).ReturnsAsync(session);

            // Act
            var result = await _controller.GetGame("abc");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(session, ok.Value);
        }

        // GetResult: null/empty/whitespace code -> BadRequest
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetResult_Returns_BadRequest_When_CodeInvalid(string? code)
        {
            // Act
            var result = await _controller.GetResult(code!);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // GetResult: not found when service returns null
        [Fact]
        public async Task GetResult_Returns_NotFound_When_ServiceReturnsNull()
        {
            // Arrange
            _gameServiceMock.Setup(s => s.GetGameResultAsync("abc")).ReturnsAsync((GameResultDto?)null);

            // Act
            var result = await _controller.GetResult("abc");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // Parameterized: GetResult behavior for different statuses
        [Theory]
        [InlineData((int)GameStatus.InProgress, "Conflict")]
        [InlineData((int)GameStatus.GameFinished, "Ok")]
        public async Task GetResult_StatusBehavior_ReturnsExpected(int statusInt, string expected)
        {
            // Arrange
            var status = (GameStatus)statusInt;
            var dto = new GameResultDto { GameCode = "abc", Status = status };
            _gameServiceMock.Setup(s => s.GetGameResultAsync("abc")).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetResult("abc");

            // Assert
            if (expected == "Ok")
            {
                var ok = Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(dto, ok.Value);
            }
            else if (expected == "Conflict")
            {
                Assert.IsType<ConflictObjectResult>(result.Result);
            }
        }

        // GetActiveGame: unauthorized when no user id
        [Fact]
        public async Task GetActiveGame_Returns_Unauthorized_When_NoUser()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetActiveGame();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        // GetActiveGame: not found when none
        [Fact]
        public async Task GetActiveGame_Returns_NotFound_When_None()
        {
            // Arrange
            _gameServiceMock.Setup(s => s.GetActiveGameByUserAsync("user1")).ReturnsAsync((GameSessionResponseDto?)null);

            // Act
            var result = await _controller.GetActiveGame();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // GetActiveGame: ok when exists
        [Fact]
        public async Task GetActiveGame_Returns_Ok_When_Exists()
        {
            // Arrange
            var session = new GameSessionResponseDto { GameCode = "abc" };
            _gameServiceMock.Setup(s => s.GetActiveGameByUserAsync("user1")).ReturnsAsync(session);

            // Act
            var result = await _controller.GetActiveGame();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(session, ok.Value);
        }

        // GetGameHistory: returns list
        [Fact]
        public async Task GetGameHistory_Returns_Ok_With_List()
        {
            // Arrange
            var list = new List<GameSummaryDto> { new GameSummaryDto { GameCode = "a" } };
            _gameServiceMock.Setup(s => s.GetGameHistoryAsync("user1")).ReturnsAsync(list);

            // Act
            var result = await _controller.GetGameHistory();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        // GetGameStatus: not found vs ok
        [Fact]
        public async Task GetGameStatus_Returns_NotFound_When_Null()
        {
            // Arrange
            _gameStatusServiceMock.Setup(s => s.GetGameStatusAsync("abc")).ReturnsAsync((GameStatusDto?)null);

            // Act
            var result = await _controller.GetGameStatus("abc");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetGameStatus_Returns_Ok_When_Found()
        {
            // Arrange
            var status = new GameStatusDto { GameCode = "abc", Status = GameStatus.InProgress };
            _gameStatusServiceMock.Setup(s => s.GetGameStatusAsync("abc")).ReturnsAsync(status);

            // Act
            var result = await _controller.GetGameStatus("abc");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(status, ok.Value);
        }
    }
}
