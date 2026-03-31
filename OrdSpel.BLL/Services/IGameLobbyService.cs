using OrdSpel.Shared.DTOs;
using System.Threading.Tasks;

namespace OrdSpel.BLL.Services
{
    public interface IGameLobbyService
    {
        Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode);
    }
}
