using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Repositories
{
    public class TurnRepository : ITurnRepository
    {
        private readonly AppDbContext _context;

        public TurnRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<GameSession?> GetSessionWithDetailsAsync(string gameCode)
        {
            //hämtar gamesession från databasen och inkluderar player och turns från samma session
            return await _context.GameSessions
                .Include(s => s.Players)
                .Include(s => s.Turns)
                .FirstOrDefaultAsync(s => s.GameCode == gameCode); //filtrerar på gamecode, hämtar första matchningen
        }

        public async Task<Word?> GetWordAsync(string text, int categoryId)
        {
            //hämtar det första ordet som matchar text och categoryid
            return await _context.Words.FirstOrDefaultAsync(w => w.Text == text && w.CategoryId == categoryId);
        }

        public async Task AddTurnAsync(GameTurn turn)
        {
            await _context.GameTurns.AddAsync(turn);
        }

        public async Task<Word?> GetRandomWordAsync(int categoryId, IEnumerable<string> excludeWords)
        {
            // Hämta ett slumpmässigt ord från kategorin som inte redan använts
            var query = _context.Words.Where(w => w.CategoryId == categoryId);

            if (excludeWords.Any())
                query = query.Where(w => !excludeWords.Contains(w.Text));

            var count = await query.CountAsync();
            if (count == 0)
                return null;

            var skip = Random.Shared.Next(count);
            return await query.Skip(skip).FirstOrDefaultAsync();
        }

        //separat spara-metod för att kunna göra flera ändringar men bara spara en gång (unit of work), kontrollerar sparandet i service-lagret
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
