using OrdSpel.Shared.UserDtos;

namespace OrdSpel.BLL.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<string?> RegisterAsync(RegisterDto dto)
        {
            // Kontrollerar att uppgifterna matchar testdatan
            if (dto.Username == "testuser" && dto.Password == "Test123!")
                return Task.FromResult<string?>("mock-token-123");

            return Task.FromResult<string?>(null);
        }
    }
}
