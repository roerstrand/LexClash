using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.Shared.AuthDTOs
{
    public class TokenResponse
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
