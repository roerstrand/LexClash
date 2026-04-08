using OrdSpel.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Interfaces
{
    public interface IGameStatusService
    {
        Task<GameStatusDto?> GetGameStatusAsync(string gameCode);
    }
}
