using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class GamePlayer
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public GameSession Session { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public int PlayerOrder { get; set; } 
        public int TotalScore { get; set; }

    }
}
