using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    class PlayerPowerUp
    {
        public enum PowerUpType { TERRAFORMER, ENGINE, CARGO, LASER, ROCKET, SHIELD}

        PowerUpType type;
        public PowerUpType Type { get { return type; } }

        int level;
        public int Level { get { return level; } }

        public PlayerPowerUp (PowerUpType type, int level)
        {
            this.type = type;
            this.level = level;
        }
    }
}
