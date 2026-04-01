namespace OrdSpel.UI.Services
{
    public class AuthStateService
    {
        public string? Token { get; private set; }
        public string? UserId { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public event Action? OnChange;

        public void Login(string token, string userId)
        {
            Token = token;
            UserId = userId;
            OnChange?.Invoke();
        }

        public void Logout()
        {
            Token = null;
            UserId = null;
            OnChange?.Invoke();
        }
    }
}