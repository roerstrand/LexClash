using OrdSpel.Shared.DTOs;
using System.Threading.Tasks;

namespace OrdSpel.BLL.Interfaces
{
    public interface IGameLobbyService
    {
        Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode);
    }
}
