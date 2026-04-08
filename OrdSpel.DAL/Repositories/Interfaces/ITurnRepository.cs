using OrdSpel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Repositories.Interfaces
{
    public interface ITurnRepository
    {
        Task<GameSession?> GetSessionWithDetailsAsync(string gameCode);
        Task<Word?> GetWordAsync(string text, int categoryId);
        Task AddTurnAsync(GameTurn turn);
        Task<Word?> GetRandomWordAsync(int categoryId, IEnumerable<string> excludeWords);
        Task SaveChangesAsync();
    }
}
