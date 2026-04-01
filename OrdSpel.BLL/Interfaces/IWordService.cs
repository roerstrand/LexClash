using OrdSpel.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Interfaces
{
    public interface IWordService
    {
        Task<List<WordDto>> GetAllAsync();
    }
}
