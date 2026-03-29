using Microsoft.AspNetCore.Identity;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.BLL.Services
{
    public interface IAuthService
    {
        Task<IdentityUser?> RegisterAsync(RegisterDto dto);
        Task<IdentityUser?> LoginAsync(LoginDto dto);
    }
}
