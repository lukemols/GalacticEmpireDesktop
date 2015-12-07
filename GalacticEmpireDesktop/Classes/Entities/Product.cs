using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    class Product
    {
        public enum ProductType { FOOD, TOOL, TECNO }

        ProductType type;
        public ProductType Type { get { return type; } }

        int quantity;
        public int Quantity { get { return quantity; } set { quantity = value; } }

        int level;
        public int Level { get { return level; } }

        public Product(ProductType type, int quantity, int level)
        {
            this.type = type;
            this.quantity = quantity;
            this.level = level;
        }

    }
}
