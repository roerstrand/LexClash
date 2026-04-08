using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrdSpel.API.Controllers;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.API.Test
{
    public class LoginTests
    {
        private Mock<SignInManager<IdentityUser>> CreateSignInManagerMock()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                null, null, null, null);

            // SignInAsync sätter cookien i ett riktigt API-anrop – i testet gör vi ingenting
            signInManagerMock
                .Setup(s => s.SignInAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>(), null))
                .Returns(Task.CompletedTask);

            return signInManagerMock;
        }

        [Fact]
        public async Task Login_OK()
        {
            var controller = new AuthController(new MockAuthService(), CreateSignInManagerMock().Object);

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
            var controller = new AuthController(new MockAuthService(), CreateSignInManagerMock().Object);

            var result = await controller.Login(new LoginDto
            {
                Username = "fel",
                Password = "fel"
            });

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
