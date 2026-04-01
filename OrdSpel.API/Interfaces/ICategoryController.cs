using Microsoft.AspNetCore.Mvc;

namespace OrdSpel.API.Interfaces
{
    public interface ICategoryController
    {
        Task<IActionResult> GetCategories();
        Task<IActionResult> GetCategoriesContent(int id);
    }
}