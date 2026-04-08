using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using OrdSpel.API.Controllers;
using Microsoft.VisualBasic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OrdSpel.Shared.DTOs;
using Xunit.Sdk;
using OrdSpel.BLL.Interfaces;

namespace OrdSpel.API.Test
{
    //testar ingen logik här, testar endast att controllern returnerar rätt svar (tex 200/400)
    public class TurnControllerTests
    {
        //mocka för att testa utan databasen
        private readonly Mock<ITurnService> _serviceMock;
        private readonly TurnController _controller;

        public TurnControllerTests()
        {
            _serviceMock = new Mock<ITurnService>();
            _controller = new TurnController(_serviceMock.Object);

            //simulerar en inloggad användare, detta görs automatiskt av asp.net och jwt i verkligeheten, men eftersom tester inte kör riktiga requests måste man själv simulera en användare med controllercontext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1") //skapar det enda claim vi behöver för testet, NameIdentifier (jwt innehåller flera claims)
            };

            //skapa nytt claimsidentity-objekt med listan av claims ovan och autentiseringstypen Test. i verkligheten ska det vara Bearer ist för Test
            var identity = new ClaimsIdentity(claims, "Test");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    //User är en inbyggd property i Controllerbase. TurnController ärver av ControllerBase
                    User = new ClaimsPrincipal(identity) //ClaimsPrincipal är ett objekt som samlar all info om den inloggade användaren
                }
            };
        }

        [Fact] //test för att kolla hur controllern reagerar när spelet går som planerat
        public async Task PostTurnSuccess()
        {
            //setup, bestämmer det som mocken ska returnera
            //när PlayTurnAsync anropas med gamecode 123, användaren och vilken requestdto som helst, inte relevant vilken i detta test så...
            _serviceMock.Setup(s => s.PlayTurnAsync("123", "user1", It.IsAny<TurnRequestDto>()))
            .ReturnsAsync((new TurnResponseDto { Score = 5 }, (string?)null)); //... returneras en ny responsedto som innehåller poäng och inget felmeddelande

            //anropa controllern, spelar ingen roll vad vi skickar in här eller ovan i mocken, eftersom vi bara kollar att controller returnerar rätt 
            var result = await _controller.PostTurn("123", new TurnRequestDto { Word = "katt" });

            Assert.IsType<OkObjectResult>(result); //förväntar att controllern returnerar 200
        }

        [Fact] //test för att kolla hur controllern reagerar när det inte är en spelares tur
        public async Task PostTurnFail()
        {
            //setup, bestämmer att mocken ska returnera att det inte är user1s tur
            _serviceMock.Setup(s => s.PlayTurnAsync("123", "user1", It.IsAny<TurnRequestDto>()))
                .ReturnsAsync(((TurnResponseDto?)null, "Det är inte din tur"));

            //anropa controllern, spelar ingen roll vad vi skickar in här eller ovan i mocken, eftersom vi bara kollar att controller returnerar rätt 
            var result = await _controller.PostTurn("123", new TurnRequestDto());

            Assert.IsType<BadRequestObjectResult>(result); //förväntar att controllern returenrar 400
        }
    }
}
