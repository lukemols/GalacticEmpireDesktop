using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    class Settlement
    {
        public enum SettlementType { MILITARY, COMMUNITY, INHABITED, CAPITAL }

        SettlementType type;
        public SettlementType Type { get { return type; } }
        
        int money;
        public int Money { get { return money; } set { if(value > 0) money = value; } }

        int inhabitants;
        public int Inhabitants { get { return inhabitants; } }

        float inhaGrowthRate;
        public float InhaGrowthRate { get { return inhaGrowthRate; } set { if (value > 0.990 && value < 1.001) inhaGrowthRate = value; } }

        float scienceLevel;
        public float ScienceLevel { get { return scienceLevel; } }

        float scienceGrowthRate;
        public float ScienceGrowthRate { get { return scienceGrowthRate; } }

        float commerceLevel;
        public float CommerceLevel { get { return commerceLevel; } }

        float commerceGrowthRate;
        public float CommerceGrowthRate { get { return commerceGrowthRate; } }

        float tecnoLevel;
        public float TecnoLevel { get { return tecnoLevel; } }

        float tecnoGrowthRate;
        public float TecnoGrowthRate { get { return tecnoGrowthRate; } }

        int defensiveArmy;
        public int DefensiveArmy { get { return defensiveArmy; } }

        int offensiveArmy;
        public int OffensiveArmy { get { return offensiveArmy; } }

        List<Product> products;
        public List<Product> Products { get { return products; } }

        public Settlement(SettlementType type, Religion.ReligionType religion)
        {
            products = new List<Product>();
            this.type = type;
            money = 20000;
            inhabitants = 50000;
            inhabitants = 100000000;//
            scienceLevel = 1;
            tecnoLevel = 1;
            commerceLevel = 1;
            defensiveArmy = 3000;
            offensiveArmy = 20;
            SetInitialRates(religion);
            scienceGrowthRate = 100f;//
            commerceGrowthRate = 100f;//
            tecnoGrowthRate = 100f;//
            inhaGrowthRate = 1.5f;//
        }

        public Settlement(SettlementType type, int money, int inhabitants, int defense, int offense, Religion.ReligionType religion)
        {
            products = new List<Product>();
            this.type = type;
            this.money = money;
            this.inhabitants = inhabitants;
            scienceLevel = 1;
            tecnoLevel = 1;
            commerceLevel = 1;
            defensiveArmy = defense;
            offensiveArmy = offense;
            SetInitialRates(religion);
        }

        private void SetInitialRates(Religion.ReligionType religion)
        {
            Random rnd = new Random(this.GetHashCode());
            inhaGrowthRate = rnd.Next(1, 10) / 10000f + 1f;
            scienceGrowthRate = rnd.Next(1, 4) / 10f;
            commerceGrowthRate = rnd.Next(1, 4) / 10f;
            tecnoGrowthRate = rnd.Next(1, 4) / 10f;

            switch(religion)
            {
                case Religion.ReligionType.ATEO:
                    commerceGrowthRate = 0.8f;
                    break;
                case Religion.ReligionType.ATOM:
                    scienceGrowthRate = 0.8f;
                    break;
                case Religion.ReligionType.CURSER:
                    tecnoGrowthRate = 0.8f;
                    break;
                case Religion.ReligionType.BLESS:
                    defensiveArmy *= 2;
                    break;
            }
        }

        public void Update()
        {
            if (type == SettlementType.CAPITAL || type == SettlementType.COMMUNITY)
            {
                if(inhabitants < 100000000) /// 100.000.000 x 10^3 (100 miliardi di persone al max)
                    inhabitants = (int)(inhabitants * inhaGrowthRate);
                scienceLevel += scienceGrowthRate;
                commerceLevel += commerceGrowthRate;
                tecnoLevel += tecnoGrowthRate;
            }
            else if (type == SettlementType.INHABITED)
            {
                inhabitants += (int)inhaGrowthRate;
                scienceLevel += scienceGrowthRate;
                commerceLevel += commerceGrowthRate;
                tecnoLevel += tecnoGrowthRate;
            }

        }

        public uint ProduceAndConsume()
        {
            int needs = (inhabitants / LevelManager.ActualLevel(scienceLevel)) / 2048;

            int food = needs / 2;
            int tecno = needs / 4;
            int tool = needs / 4;

            uint retNeeds = (uint)needs;
            
            int i = 0;
            do
            {
                if (i >= Products.Count)
                {
                    Product p1 = new Product(Product.ProductType.FOOD, food / 4, LevelManager.ActualLevel(scienceLevel));
                    Product p2 = new Product(Product.ProductType.TECNO, tecno / 4, LevelManager.ActualLevel(tecnoLevel));
                    Product p3 = new Product(Product.ProductType.TOOL, tool / 4, LevelManager.ActualLevel(tecnoLevel));
                    needs = 0;

                    products.Add(p1);
                    products.Add(p2);
                    products.Add(p3);
                }
                else
                {
                    if(products[i].Type == Product.ProductType.FOOD)
                    {
                        if(food > products[i].Quantity)
                        {
                            food -= products[i].Quantity;
                            needs -= products[i].Quantity;
                            products[i].Quantity = 0;
                        }
                        else
                        {
                            products[i].Quantity -= food;
                            needs -= food;
                            food = 0;
                        }
                    }
                    else if (products[i].Type == Product.ProductType.TECNO)
                    {
                        if (tecno > products[i].Quantity)
                        {
                            tecno -= products[i].Quantity;
                            needs -= products[i].Quantity;
                            products[i].Quantity = 0;
                        }
                        else
                        {
                            products[i].Quantity -= tecno;
                            needs -= tecno;
                            tecno = 0;
                        }
                        int diff = products[i].Level - LevelManager.ActualLevel(tecnoLevel);
                        if (diff > 0)
                            tecnoLevel += 0.3f * diff;
                    }
                    else
                    {
                        if (tool > products[i].Quantity)
                        {
                            tool -= products[i].Quantity;
                            needs -= products[i].Quantity;
                            products[i].Quantity = 0;
                        }
                        else
                        {
                            products[i].Quantity -= tool;
                            needs -= tool;
                            food = 0;
                        }
                        int diff = products[i].Level - LevelManager.ActualLevel(tecnoLevel);
                        if (diff > 0)
                            tecnoLevel += 0.3f * diff;
                    }
                }
                i++;
            }
            while (needs != 0);

            for (int j = products.Count - 1; j >= 0; j--)
            {
                if (products[j].Quantity == 0)
                    products.RemoveAt(j);
            }
            
            return retNeeds;
        }
    }
}
