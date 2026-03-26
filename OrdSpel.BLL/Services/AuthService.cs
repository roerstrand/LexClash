using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OrdSpel.Shared.UserDtos;
//using OrdSpel.Shared.UserDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;




namespace OrdSpel.BLL.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Username };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return null;

            // 2. Generera och returnera JWT-token
            return GenerateJwtToken(user);
        }

        public async Task<bool> LoginAsync(LoginDTO dto) //Kolla namngivning sen mot Login!
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DIN-HEMLIGA-NYCKEL"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
