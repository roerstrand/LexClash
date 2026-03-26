using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class GamePlayer
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public GameSession Session { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public int PlayerOrder { get; set; } // 1 or 2
        public int TotalScore { get; set; } = 0;

    }
}
