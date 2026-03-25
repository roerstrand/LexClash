using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Word> Words { get; set; } = new List<Word>();
        public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    }
}
