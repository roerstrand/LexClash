using Microsoft.AspNetCore.Identity;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ServiceResult<IdentityUser>> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Username);
            if (existing != null)
                return ServiceResult<IdentityUser>.Fail("Användarnamnet är redan taget.");

            var user = new IdentityUser { UserName = dto.Username };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return ServiceResult<IdentityUser>.Fail("Något gick fel vid registrering.");

            return ServiceResult<IdentityUser>.Ok(user);
        }

        public async Task<IdentityUser?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return null;

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            return passwordOk ? user : null;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
