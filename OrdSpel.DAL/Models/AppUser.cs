using Microsoft.AspNetCore.Identity;

namespace OrdSpel.DAL.Models;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }

    public ICollection<GamePlayer> GamePlayers { get; set; } = new List<GamePlayer>();
    public ICollection<GameTurn> GameTurns { get; set; } = new List<GameTurn>();
}
