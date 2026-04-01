using Microsoft.AspNetCore.Identity;
using OrdSpel.Shared;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<IdentityUser>> RegisterAsync(RegisterDto dto);
        Task<IdentityUser?> LoginAsync(LoginDto dto);
        Task<bool> DeleteAsync(string userId);
    }
}
