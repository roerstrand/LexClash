using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class GameTurn
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public GameSession Session { get; set; } = null!;
        public int Round { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string? Word { get; set; }
        public int Score { get; set; }
        public bool PassedTurn { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
