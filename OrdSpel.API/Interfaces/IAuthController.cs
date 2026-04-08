using Microsoft.AspNetCore.Mvc;
using OrdSpel.Shared.AuthDTOs;

namespace OrdSpel.API.Interfaces
{
    public interface IAuthController
    {
        Task<IActionResult> Delete();
        Task<IActionResult> Login(LoginDto dto);
        Task<IActionResult> Logout();
        IActionResult Me();
        Task<IActionResult> Register([FromBody] RegisterDto dto);
    }
}