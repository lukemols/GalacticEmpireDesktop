using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GalacticEmpire
{
    class WarShip : Ship
    {
        int offensivePower;
        public int OffensivePower { get { return offensivePower; } }

        public WarShip(Vector3 initialPosition, int life = 500, int speed = 5, int offensive = 10) : base(initialPosition, life, speed)
        {
            offensivePower = offensive;
        }
    }
}
