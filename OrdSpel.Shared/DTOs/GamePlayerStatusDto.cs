namespace OrdSpel.Shared.DTOs
{
    public sealed record GamePlayerStatusDto(
        string UserId,
        int PlayerOrder,
        int TotalScore);
}
