namespace OrdSpel.UI.Services
{
    public class AuthStateService
    {
        public string? CookieValue { get; private set; }
        public string? UserId { get; private set; }
        public string? Username { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(CookieValue);

        public event Action? OnChange;

        public void Login(string cookieValue, string? userId = null, string? username = null)
        {
            CookieValue = cookieValue;
            UserId = userId;
            Username = username;
            OnChange?.Invoke();
        }

        public void Logout()
        {
            CookieValue = null;
            UserId = null;
            Username = null;
            OnChange?.Invoke();
        }
    }
}
