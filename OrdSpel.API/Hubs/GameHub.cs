using Microsoft.AspNetCore.SignalR;

namespace OrdSpel.API.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGame(string gameCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        }
    }
}
