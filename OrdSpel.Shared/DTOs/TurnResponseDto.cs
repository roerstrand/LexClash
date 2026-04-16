namespace OrdSpel.Shared.DTOs
{
    public class TurnResponseDto
    {
        public int Score { get; set; }
        public int TotalScore { get; set; }
        public string NextUserId { get; set; } = string.Empty;
        public int CurrentRound { get; set; }
        public bool GameFinished { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
