using System;
using System.Collections.Generic;
using System.Text;
using OrdSpel.Shared.Enums;

namespace OrdSpel.Shared.GameDTOs
{
    public class GameSessionResponseDto
    {
        public string GameCode { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public int CategoryId { get; set; }
        public string StartWord { get; set; } = string.Empty;
        public List<string> PlayerIds { get; set; } = new();
    }
}
