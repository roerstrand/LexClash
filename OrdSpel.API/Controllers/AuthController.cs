using Microsoft.AspNetCore.Mvc;
using OrdSpel.BLL.Services;

namespace OrdSpel.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
    }
}
