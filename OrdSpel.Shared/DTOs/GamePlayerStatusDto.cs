namespace OrdSpel.Shared.DTOs
{
    public sealed record GamePlayerStatusDto(
        string UserId,
        string? Username,
        int PlayerOrder,
        int TotalScore);
}
