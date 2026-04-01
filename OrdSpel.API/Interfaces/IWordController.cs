using Microsoft.AspNetCore.Mvc;

namespace OrdSpel.API.Interfaces
{
    public interface IWordController
    {
        Task<IActionResult> GetAllWords();
    }
}