namespace OrdSpel.BLL.Interfaces
{
    public interface IUserNameResolver
    {
        Task<string?> GetUsernameAsync(string userId);
        Task<Dictionary<string, string>> GetUsernamesAsync(IEnumerable<string> userIds);
    }
}
