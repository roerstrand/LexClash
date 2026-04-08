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
            //hämta nuvarande session
            var session = await _turnRepository.GetSessionWithDetailsAsync(gameCode);

            if (session == null)
            {
                return (null, "Spelet hittades inte"); //hårdkodade felmeddelande, vill ändra till enums om det finns tid
            }

            if (session.Status != GameStatus.InProgress)
            {
                return (null, "Spelet är inte aktivt");
            }

            if (session.CurrentTurnUserId != userId)
            {
                return (null, "Det är inte din tur");
            }

            int score;

            //om en spelare väljer att passa
            if (dto.PassedTurn)
            {
                score = GameRules.PassPenalty; // -5p för att passa
            }
            else
            {
                var word = dto.Word?.Trim().ToLower(); //.trim() tar bort mellanslag före eller efter ordet

                if (string.IsNullOrEmpty(word))
                {
                    return (null, "Inget ord angivet");
                }

                var lastWord = session.Turns.Any() //kollar om det finns några tidigare turns att hämta senaste ord från, om inte är det första rundan annars:
                             ? session.Turns.OrderByDescending(t => t.CreatedAt).First().Word //om det finns turns sortera på den senaste (descending, first) och hämta senaste ordet
                             ?? session.StartWord //om det inte finns tidigare ord, använd startord
                             : session.StartWord; //om det inte finns några tidigare turns eller om det är första rundan, använd startord var lastWord = session.StartWord

                //jämför första bokstaven i det nya ordet med sista bokstaven i föregående ord. ^1 räkna bakifrån, första tecknet från slutet
                if (word[0] != lastWord[^1])
                {
                    return (null, $"Ordet måste börja på sista bokstaven av föregående ord '{lastWord[^1]}'.");
                }

                //kolla om ordet finns i den specifika kategorin via repository-metoden
                var wordInCategory = await _turnRepository.GetWordAsync(word, session.CategoryId);

                if (wordInCategory == null)
                {
                    return (null, "Ordet finns inte i den valda kategorin");
                }

                var usedWords = session.Turns.Select(t => t.Word).ToHashSet(); //samlar alla ord från tidigare turns i en hashset. hashset är snabbare för att kolla efter dubletter. en hashset kan inte innehålla dubletter

                if (usedWords.Contains(word))
                {
                    return (null, "Ordet har redan använts i denna spelomgång.");
                }

                //.sum är en metod som loopar igenom alla tecken (c) i ett ord och summerar poängen för ordet. Använder metoden GetScores i LetterScores som innehåller dictionaryn med bestämda poäng för varje bokstav
                score = word.Sum(c => LetterScores.GetScore(c));

                //det ovan är samma sak som:

                //int score = 0;

                //foreach (char c in word)
                //{
                //    score += LetterScores.GetScore(c);
                //}

                //om ordets längd är längre än eller lika med den bestämda längden för bonuspoäng (text 15 karaktärer långt)
                if (word.Length >= GameRules.LongWordThreshold)
                {
                    score += GameRules.LongWordBonus; //tilldela bonuspoäng för ordet
                }

                //om ordet är klasssat som svårt
                if (wordInCategory.IsHard)
                {
                    score += GameRules.HardWordBonus; //tilldela bonuspoäng för ordet
                }

                //skapa en ny turn med det nya ordet, normaliserat/rensat/städat till små bokstäver, inget ev mellanslag
                dto.Word = word;
            }

            //spara innehållet från den nya turen i databasen, använder då modell ist för dto
            var turn = new GameTurn
            {
                SessionId = session.Id,
                Round = session.CurrentRound,
                UserId = userId,
                Word = dto.PassedTurn ? null : dto.Word,
                Score = score,
                PassedTurn = dto.PassedTurn,
                CreatedAt = DateTime.UtcNow
            };

            //spara turen via repository-metoden
            await _turnRepository.AddTurnAsync(turn);

            //hitta användaren som spelar just nu och uppdatera dess poäng
            var player = session.Players.First(p => p.UserId == userId);
            player.TotalScore += score;

            //byt till andra spelarens tur
            var nextPlayer = session.Players.First(p => p.UserId != userId);
            session.CurrentTurnUserId = nextPlayer.UserId; //byt till nästa spelare
            session.CurrentRound++; //öka round med 1

            //kolla om vi nått spelets slut (20 rundor), om den nuvarande rundan är större än totala antalet rundor
            bool gameIsFinished = session.CurrentRound > GameRules.TotalRounds;

            //om det är sista rundan, sätt spelets status som klart
            if(gameIsFinished)
            {
                session.Status = GameStatus.GameFinished;
            }

            await _turnRepository.SaveChangesAsync();

            //returnera responsen för denna turn att använda i controllern
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
