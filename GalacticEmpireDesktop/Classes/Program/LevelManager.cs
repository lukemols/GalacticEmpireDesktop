using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    static class LevelManager
    {
        static float[] levels = { 0, 800, 1800, 3000, 4200, 5500, 6800, 8400, 10000, 12500 };
        
        static public int ActualLevel(float levelValue)
        {
            for(int i = 0; i < levels.Length; i++)
            {
                if (levelValue < levels[i])
                    return i;
            }
            return 10; //se va oltre, ritorna il massimo livello possibile
        }

        static public float PercentLevel(float levelValue)
        {
            int actual = ActualLevel(levelValue);
            if (actual == 10 || actual == 0)
                return 1f;
            return (levelValue - levels[actual - 1]) / (levels[actual] - levels[actual - 1]);
        }
    }
}
