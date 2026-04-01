using OrdSpel.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.Shared.DTOs
{
    public class GameLobbyStatusDto
    {
        public int SessionId { get; set; }
        public string GameCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string StartWord { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public int PlayerCount { get; set; }
        public int MaxPlayers { get; set; } = 2;
        public bool IsReadyToStart { get; set; }
        public string? CurrentTurnUserId { get; set; }
    }
}
