using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    static class GameParams
    {
        public enum Difficulty { EASY, NORMAL, HARD }
        public static int starNumber;
        public static int empireNumber;
        public static Religion.ReligionType religionType;
        public static string name;
        public static Difficulty gameDifficulty;
    }
}
