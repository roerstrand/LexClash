using OrdSpel.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Services
{
    public interface IGameLobbyService
    {
        Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode);
    }
}
