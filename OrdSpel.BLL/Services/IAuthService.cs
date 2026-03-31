using Microsoft.AspNetCore.Identity;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.BLL.Services
{
    public interface IAuthService
    {
        Task<IdentityUser?> RegisterAsync(RegisterDto dto);
        Task<IdentityUser?> LoginAsync(LoginDto dto);
    }
}
