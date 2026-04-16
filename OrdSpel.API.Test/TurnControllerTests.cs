using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using OrdSpel.API.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OrdSpel.Shared.DTOs;
using OrdSpel.BLL.Interfaces;
using OrdSpel.API.Hubs;

namespace OrdSpel.API.Test
{
    public class TurnControllerTests
    {
        private readonly Mock<ITurnService> _serviceMock;
        private readonly Mock<IHubContext<GameHub>> _hubContextMock;
        private readonly TurnController _controller;

        public TurnControllerTests()
        {
            _serviceMock = new Mock<ITurnService>();
            _hubContextMock = new Mock<IHubContext<GameHub>>();
            var clientsMock = new Mock<IHubClients>();
            var groupMock = new Mock<IClientProxy>();

            groupMock.Setup(g => g.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(groupMock.Object);
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            _controller = new TurnController(_serviceMock.Object, _hubContextMock.Object);

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

        [Fact]
        public async Task PostTurnSuccess()
        {
            _serviceMock.Setup(s => s.PlayTurnAsync("123", "user1", It.IsAny<TurnRequestDto>()))
            .ReturnsAsync((new TurnResponseDto { Score = 5 }, (string?)null));

            var result = await _controller.PostTurn("123", new TurnRequestDto { Word = "katt" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PostTurnFail()
        {
            _serviceMock.Setup(s => s.PlayTurnAsync("123", "user1", It.IsAny<TurnRequestDto>()))
                .ReturnsAsync(((TurnResponseDto?)null, "It is not your turn"));

            var result = await _controller.PostTurn("123", new TurnRequestDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
