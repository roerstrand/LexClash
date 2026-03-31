using Microsoft.AspNetCore.Mvc;
using OrdSpel.API.Controllers;
using OrdSpel.API.Services;
using OrdSpel.BLL.Services;
using Microsoft.Extensions.Configuration;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.API.Test
{

    public class LoginTests
    {
        private IConfiguration FakeConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "12345678901234567890123456789012" },
                    { "Jwt:Issuer", "test" },
                    { "Jwt:Audience", "test" }
                })
                .Build();
        }

        [Fact]
        public async Task Login_OK()
        {
            var controller = new AuthController(
                new MockAuthService(),
                new JwtService(FakeConfig())
            );

            var result = await controller.Login(new LoginDto
            {
                Username = "testuser",
                Password = "Test123!"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_Fail()
        {
            var controller = new AuthController(
                new MockAuthService(),
                new JwtService(FakeConfig())
            );

            var result = await controller.Login(new LoginDto
            {
                Username = "fel",
                Password = "fel"
            });

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
