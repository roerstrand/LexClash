using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.Shared.Constraints
{
    //förbestämda poäng för varje bokstav
    //dictionary efetrsom det är lätt att slå upp varje bokstav för att få fram dess värde
    public static class LetterScores
    {
        public static readonly Dictionary<char, int> Scores = new()
        {
            {'a',1},{'b',2},{'c',3},{'d',2},{'e',1},{'f',3},
            {'g',2},{'h',3},{'i',1},{'j',4},{'k',2},{'l',1},
            {'m',2},{'n',1},{'o',1},{'p',3},{'q',10},{'r',1},
            {'s',1},{'t',1},{'u',1},{'v',3},{'w',4},{'x',8},
            {'y',4},{'z',4},{'å',4},{'ä',4},{'ö',4}
        };

        //metod som slår upp poängen för varje bokstav, om bokstaven/tecknet inte finns returneras 0
        public static int GetScore(char letter) =>
            Scores.TryGetValue(char.ToLower(letter), out var score) ? score : 0;
    }
}
