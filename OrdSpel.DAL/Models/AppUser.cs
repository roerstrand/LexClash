using Microsoft.AspNetCore.Identity;

namespace OrdSpel.DAL.Models
{
    public class AppUser : IdentityUser
    {
        // Additional profile properties can go here
        public string DisplayName { get; set; } = string.Empty;
    }
}
