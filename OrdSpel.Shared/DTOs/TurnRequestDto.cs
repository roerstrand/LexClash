
namespace OrdSpel.Shared.DTOs
{
    public class TurnRequestDto
    {
        //dto som skickar data från frontend/UI/client till backend/api/server när en spelare gjort sitt drag
        //2 scenarion: 
        //1 - spelaren skriver ett ord, word får ett värde och passedturn blir false
        //2 - spelaren passar, word blir null, passedturn blir true
        public string? Word { get; set; } // ? = kan vara null
        public bool PassedTurn { get; set; }
    }
}
