using Microsoft.AspNetCore.Identity;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.BLL.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<IdentityUser?> RegisterAsync(RegisterDto dto)
        {
            // Kontrollerar att uppgifterna matchar testdatan
            if (dto.Username == "testuser" && dto.Password == "Test123!")
                return Task.FromResult<IdentityUser?>(new IdentityUser { UserName = dto.Username });

            return Task.FromResult<IdentityUser?>(null);
        }

        public Task<IdentityUser?> LoginAsync(LoginDto dto)
        {
            if (dto.Username == "testuser" && dto.Password == "Test123!")
                return Task.FromResult<IdentityUser?>(new IdentityUser { UserName = dto.Username });

            return Task.FromResult<IdentityUser?>(null);
        }
    }
}
