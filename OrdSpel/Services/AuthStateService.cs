namespace OrdSpel.UI.Services
{
    public class AuthStateService
    {
        public string? CookieValue { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(CookieValue);

        public event Action? OnChange;

        public void Login(string cookieValue)
        {
            CookieValue = cookieValue;
            OnChange?.Invoke();
        }

        public void Logout()
        {
            CookieValue = null;
            OnChange?.Invoke();
        }
    }
}
