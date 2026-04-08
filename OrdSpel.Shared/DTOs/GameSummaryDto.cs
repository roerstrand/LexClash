using OrdSpel.Shared.Enums;

namespace OrdSpel.Shared.DTOs;

public sealed record GameSummaryDto
{
    public string GameCode { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<GamePlayerStatusDto> Players { get; init; } = [];
    public string? WinnerUserId { get; init; }
    public string? WinnerUsername { get; init; }
}
