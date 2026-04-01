using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.UI.Interfaces
{
    public interface IHttpService
    {
        HttpClient _httpClient { get; }

        Task<AuthResult> LoginUser(LoginDto loginDto);
        Task<AuthResult> RegisterUser(RegisterDto dto);
        void SetBearerToken(string token);
    }
}