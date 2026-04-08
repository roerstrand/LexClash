using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.Shared.Constraints
{
    //förbestämda regler för spelet, antalet rundor, poängavdrag, bonuspoäng
    public static class GameRules
    {
        public const int TotalRounds = 20;
        public const int PassPenalty = -5; //minuspoäng för att passa
        public const int LongWordBonus = 3; //bonuspoäng för långa ord, ges vid gränsen nedan
        public const int LongWordThreshold = 12; //gränsen för när bonusen ovan ska ges, alltså om ett ord är längre än 12 bokstäver får man longwordbonus; 3p
        public const int HardWordBonus = 3; //används just nu inte eftersom vi ännu inte har tilldelat något ord isHard = true
    }
}
