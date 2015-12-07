using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameCommerceManager
    {
        static Planet actualPlanet;
        public static Planet ActualPlanet { get { return actualPlanet; } }

        public static void EnterPlanet(Planet planet)
        {
            actualPlanet = planet;
        }

        public static int GetRepairPrize()
        {
            if (actualPlanet == GameManager.PlayerCapital)
                return 0;
            int i = PlayerShip.MaxLife - PlayerShip.Life;
            if (i == 0)
                return 0;
            else
            {
                i *= 10;
                int lvlDiff = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.CommerceLevel) - LevelManager.ActualLevel(GameManager.PlayerCapital.PlanetSettlement.CommerceLevel);
                if (lvlDiff <= 0)
                    i /= 2;
                else
                    i *= 2;
                if (GameManager.ActualSystemOwner.EmpireName == GameManager.PlayerEmpire.EmpireName)
                    i /= 4;
            }
            return i;
        }

        public static int GetRechargePrize()
        {
            if (actualPlanet == GameManager.PlayerCapital)
                return 0;
            int i = (int)(PlayerShip.MaxEnergy - PlayerShip.Energy);
            if (i == 0)
                return 0;
            else
            {
                i *= 10;
                int lvlDiff = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.CommerceLevel) - LevelManager.ActualLevel(GameManager.PlayerCapital.PlanetSettlement.CommerceLevel);
                if (lvlDiff <= 0)
                    i /= 2;
                else
                    i *= 2;
                if (GameManager.ActualSystemOwner.EmpireName == GameManager.PlayerEmpire.EmpireName)
                    i /= 4;
                if (GameManager.ActualSystemOwner.ShowRelationWithPlayer().RelationPoints < 0)
                    i *= 2;
                else
                    i /= 2;
            }
            return i;
        }
    }
}
