namespace OrdSpel.UI.Models
{
    public class AuthResult //Modell för att hantera resultatet av en autentiseringsförsök, inklusive om det lyckades, den genererade token och eventuella felmeddelanden.
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
