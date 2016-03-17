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


        public CommercialShip(Vector3 initialPosition, int life = 500, int speed = 5, int maxCargo = 1000, int MaxLife = 500)
            : base(initialPosition, life, speed, MaxLife)
        {
            products = new List<Product>();
            this.maxCargo = maxCargo;
        }

        public CommercialShip(Vector3 initialPosition, Vector3 targetPosition, List<Product> products, int life = 500, int speed = 5, int maxCargo = 1000, int MaxLife = 500)
            : base(initialPosition, targetPosition, life, speed, MaxLife)
        {
            this.products = products;
            this.maxCargo = maxCargo;
        }
    }
}
