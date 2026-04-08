using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace OrdSpel.API.Hubs
{
    //en hub notifierar klienter när serverns state förändras, så att ui kan uppdateras och spelare ska få spelet uppdaterat i realtid
    public class GameHub : Hub
    {
        public async Task JoinGame(string gameCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        }
    }
}
