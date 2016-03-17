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
        public static int autosaveNumber = 3;
        public static int autosaveMinutes = 1;
        public static bool musicEnabled = true;
        public static bool soundEnabled = true;
    }
}
