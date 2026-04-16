using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Data.SeededData
{
    public static class SeededUserData
    {
        public static async Task SeedUserAsync(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByNameAsync("spelare1") == null)
            {
                var user = new IdentityUser { UserName = "spelare1", EmailConfirmed = true };
                await userManager.CreateAsync(user, "123");
            }

            if (await userManager.FindByNameAsync("spelare2") == null)
            {
                var user = new IdentityUser { UserName = "spelare2", EmailConfirmed = true };
                await userManager.CreateAsync(user, "123");
            }

            if (await userManager.FindByNameAsync("playwright_user") == null)
            {
                var user = new IdentityUser { UserName = "playwright_user", EmailConfirmed = true };
                await userManager.CreateAsync(user, "Test123!");
            }
        }
    }
}
