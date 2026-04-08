using Microsoft.AspNetCore.Identity;
using OrdSpel.BLL.Interfaces;

namespace OrdSpel.API.Services
{
    public class UserNameResolver : IUserNameResolver
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserNameResolver(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> GetUsernameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName;
        }

        public async Task<Dictionary<string, string>> GetUsernamesAsync(IEnumerable<string> userIds)
        {
            var result = new Dictionary<string, string>();
            foreach (var id in userIds.Distinct())
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user?.UserName != null)
                    result[id] = user.UserName;
            }
            return result;
        }
    }
}
