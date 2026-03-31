namespace OrdSpel.UI.Services
{
    public class AuthStateService
    {
        public string? Token { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public event Action? OnChange;

        public void Login(string token)
        {
            Token = token;
            OnChange?.Invoke();
        }

        public void Logout()
        {
            Token = null;
            OnChange?.Invoke();
        }
    }
}