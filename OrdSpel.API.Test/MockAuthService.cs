using Microsoft.AspNetCore.Identity;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.API.Test
{
    public class MockAuthService : IAuthService
    {
        public Task<ServiceResult<IdentityUser>> RegisterAsync(RegisterDto dto)
        {
            if (dto.Username == "testuser" && dto.Password == "Test123!")
                return Task.FromResult(ServiceResult<IdentityUser>.Ok(new IdentityUser { UserName = dto.Username }));

            return Task.FromResult(ServiceResult<IdentityUser>.Fail("Username is already taken."));
        }

        public Task<IdentityUser?> LoginAsync(LoginDto dto)
        {
            if (dto.Username == "testuser" && dto.Password == "Test123!")
                return Task.FromResult<IdentityUser?>(new IdentityUser { UserName = dto.Username });

            return Task.FromResult<IdentityUser?>(null);
        }

        public Task<bool> DeleteAsync(string userId)
        {
            return Task.FromResult(true);
        }
    }
}
