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
    //testar logik i service-tester
    public class TurnServiceTests
    {
        //använd mockdata i tester
        //Skapa en mock av ITurnRepository för att testa utan riktig databas
        private readonly Mock<ITurnRepository> _repoMock;
        private readonly TurnService _turnService;

        public TurnServiceTests()
        {
            _repoMock = new Mock<ITurnRepository>();
            _turnService = new TurnService(_repoMock.Object);
        }

        [Fact] //test som kontrollerar att gamesession funkar som det ska
        public async Task PlayTurnAsync_SessionSuccess()
        {
            //setup, skapa en ny session att använda i testet
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

            //returnerar gamesessionen ovan när GetSessionWithDetailsAsync anropas med gamecode ABC
            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("ABC"))
                .ReturnsAsync(session);

            //simulerar att databasen innehåller ordet "tiger" i kategori 1 så att spelaren kna välja tiger som input
            _repoMock.Setup(r => r.GetWordAsync("tiger", 1))
                .ReturnsAsync(new Word
                {
                    Text = "tiger",
                    CategoryId = 1,
                    IsHard = false
                });

            //simulerar en spelares input  
            var dto = new TurnRequestDto
            {
                Word = "tiger",
                PassedTurn = false
            };

            //använder metoden PlayTurnAsync tillsammans med gamecode, användarens id och användarens ord för att få tillbaka response eller error
            var (response, error) = await _turnService.PlayTurnAsync("ABC", "user1", dto);

            // testen
            Assert.Null(error); //förväntar att error ska vara null (vi förväntar oss att detta testa ska vara succuessful)
            Assert.NotNull(response); //förväntar att response inte ska vara null
            Assert.Equal("user2", response.NextUserId); //förväntar att användare 2 ska vara samma som nästa användare eftersom det nyss har varit user1 som spelat
            Assert.Equal(2, response.CurrentRound); //spelet börjar på 1 (se start av test) och i metoden PlayTurnAsync ökar currentround med 1 efetr varje spelare har spelat/stått över sin tur och nu är det spelare2s tur så det är runda 2
        }

        [Fact]
        public async Task PlayTurnAsync_SessionFail()
        {
            //setup
            //använder fel gamecode, simulerar att det inte finns en session med gamecode 123
            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
                .ReturnsAsync((GameSession?)null);

            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", new TurnRequestDto());

            //test
            Assert.Null(response); //ska inte finnas en session med gamecode 1234
            Assert.Equal("Spelet hittades inte", error); //förväntar att error message ska vara samma som "Spelet hittades inte"
        }

        [Fact] //test för att kontrollera att det måste vara rätt spelare som spelar
        public async Task PlayTurnAsync_WrongPlayer()
        {
            //setup, skapa ny session. behöver inte använda alla fält, kan välja endast de som är relevanta för testet
            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user2"
            };

            //returnera session ovan när metoden anropas med gamecode 123
            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
                .ReturnsAsync(session);

            //får tillbaka response och error, skickar medvetet in fel currentTurnUserId
            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", new TurnRequestDto());

            Assert.Null(response);
            Assert.NotNull(error);
            Assert.Equal("Det är inte din tur", error);
        }

        [Fact] //kontrollerar att det blir andra spelarens tur efter man har passat
        public async Task PlayTurnAsync_PassedTurn()
        {
            //setup med relevanta fält
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

            //returnera session ovan när metoden anropas med gamecode 123
            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123"))
               .ReturnsAsync(session);

            //användaren user1 passar sin tur
            var dto = new TurnRequestDto
            {
                PassedTurn = true
            };

            //får tillbaka response och error
            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            Assert.Null(error);
            Assert.NotNull(response);
            Assert.Equal(GameRules.PassPenalty, response.Score); //minuspoäng
            Assert.Equal("user2", response.NextUserId);
            Assert.NotEqual("user1", response.NextUserId);
        }

        [Fact]
        public async Task PlayTurnAsync_WordNotstartingWithCorrectLetter()
        {
            //setup
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

            //användaren svarar med ett ord som inte börjar på rätt bokstav
            var dto = new TurnRequestDto
            {
                Word = "tiger"
            };

            //får tillbaka response och error efter vi har skickat med gamecode, användarid och turnrequestdton i metoden
            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            Assert.NotNull(error);
            Assert.Null(response);
            Assert.Equal($"Ordet måste börja på sista bokstaven av föregående ord 'n'.", error);
        }

        [Fact]
        public async Task PlayTurnAsync_LongWordBonus()
        {
            var longWord = "schalottenlök"; 

            var session = new GameSession
            {
                Status = GameStatus.InProgress,
                CurrentTurnUserId = "user1",
                StartWord = "potatis", 
                Turns = new List<GameTurn>(),
                Players = new List<GamePlayer>
            {
                new GamePlayer { UserId = "user1" },
                new GamePlayer { UserId = "user2" }
            }
            };

            _repoMock.Setup(r => r.GetSessionWithDetailsAsync("123")).ReturnsAsync(session);
            _repoMock.Setup(r => r.GetWordAsync(longWord, It.IsAny<int>())) //skickar in det långa ordet, spelar ingen roll vilken int som skickas in tillsammans med ordet
                     .ReturnsAsync(new Word { Text = longWord, IsHard = false }); //returnera nytt word-objket

            //nytt ord från användaren 
            var dto = new TurnRequestDto 
            { 
                Word = longWord 
            };

            //ta emot response och error från sessionen, via PlayTurnAsync i turnservice
            var (response, error) = await _turnService.PlayTurnAsync("123", "user1", dto);

            //hämta varje bokstavs individuella värde och summera ordets totala värde. c är en bokstav i ordet (char)
            var expectedWordScore = longWord.Sum(c => LetterScores.GetScore(c));

            //förväntar att ordets poäng + extrapoäng för långt ord ska vara samma som response poängen
            Assert.Equal(expectedWordScore + GameRules.LongWordBonus, response?.Score); 
        }
    }
}
