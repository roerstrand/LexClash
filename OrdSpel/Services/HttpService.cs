using Microsoft.AspNetCore.Mvc;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.UI.Services
{
    public class HttpService
    {
        public HttpClient HttpClient { get; }


        public async Task<ActionResult> LoginUser(LoginDTO loginDto)
        {
            try
            {
                // endpoint auth existerar inte ännu i api som controller.
                var result = await HttpClient.PostAsJsonAsync("api/auth", loginDto);
                return new OkObjectResult(result);

            } catch (Exception ex)
            {
                // Handle exceptions
                return new BadRequestObjectResult(ex.Message);
            }

            
        }

        public void RegisterUser(string username, string password, string email)
        {
            // Implement registration logic here
        }
    }
}
