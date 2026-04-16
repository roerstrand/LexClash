using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrdSpel.Shared.AuthDTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters.")]
        public string Password { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
