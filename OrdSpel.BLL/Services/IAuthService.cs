using OrdSpel.Shared.UserDtos;

namespace OrdSpel.BLL.Services
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
    }
}
