using OrdSpel.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public string GameCode { get; set; } = string.Empty;
        public GameStatus Status { get; set; } = GameStatus.WaitingForPlayers;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string StartWord { get; set; } = string.Empty;
        public int CurrentRound { get; set; }
        public string? CurrentTurnUserId { get; set; }
        public DateTime CreatedAt { get; set; }

   
        public ICollection<GamePlayer> Players { get; set; } = new List<GamePlayer>();
        public ICollection<GameTurn> Turns { get; set; } = new List<GameTurn>();

    }
}
