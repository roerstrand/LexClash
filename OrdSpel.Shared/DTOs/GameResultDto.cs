using OrdSpel.Shared.Enums;

namespace OrdSpel.Shared.DTOs;

public sealed record GameResultDto
{
    public string GameCode { get; init; } = string.Empty;
    public GameStatus Status { get; init; }
    public IReadOnlyList<GamePlayerStatusDto> Players { get; init; } = [];
    public string? WinnerUserId { get; init; }
}
