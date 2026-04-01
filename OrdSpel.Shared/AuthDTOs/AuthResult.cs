namespace OrdSpel.Shared.AuthDTOs
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
