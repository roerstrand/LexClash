using Microsoft.AspNetCore.Identity;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser?> RegisterAsync(RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Username };
            var result = await _userManager.CreateAsync(user, dto.Password);

            return result.Succeeded ? user : null;
        }

        public async Task<IdentityUser?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return null;

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            return passwordOk ? user : null;
        }
    }
}
