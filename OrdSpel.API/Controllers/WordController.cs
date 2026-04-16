using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using OrdSpel.API.Interfaces;
using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Repositories.Interfaces;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase, IWordController
    {
        private readonly IWordService _wordService;

        public WordController(IWordService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWords()
        {
            var words = await _wordService.GetAllAsync();
            return Ok(words);
        }
    }
}
