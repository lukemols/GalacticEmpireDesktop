using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GalacticEmpire
{
    class CommercialShip : Ship
    {
        List<Product> products;
        public List<Product> Products { get { return products; } }

        int maxCargo;
        public int MaxCargo { get { return maxCargo; } }


        public CommercialShip(Vector3 initialPosition, int life = 500, int speed = 5, int maxCargo = 1000) : base(initialPosition, life, speed)
        {
            products = new List<Product>();
            this.maxCargo = maxCargo;
        }
    }
}
