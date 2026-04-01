
using OrdSpel.Shared.Enums;
using System.Collections.Generic;

namespace OrdSpel.Shared.DTOs
{
    public sealed record GameStatusDto
    {
        public int SessionId { get; init; }
        public string GameCode { get; init; } = string.Empty;
        public GameStatus Status { get; init; }

        public int CurrentRound { get; init; }
        public string? CurrentTurnUserId { get; init; }

        public string StartWord { get; init; } = string.Empty;
        public string? LastWord { get; init; }

        public IReadOnlyList<GamePlayerStatusDto> Players { get; init; } = [];
        //public IReadOnlyList<GameTurnDto> Turns { get; init; } = [];
    }
}
