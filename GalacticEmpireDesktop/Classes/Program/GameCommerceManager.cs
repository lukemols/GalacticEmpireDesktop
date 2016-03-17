using System;
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

        static public int GetProductBuyPrize(Product product)
        {
            int i = (int)(product.Level * 5 * GetDifficultyMultiplier(true));
            if (actualPlanet == GameManager.PlayerCapital)
                return i;
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

            if (i <= 0)
                i = 1;
            return i;
        }

        static public int GetProductSellPrize(Product product)
        {
            int i = (int)(product.Level * 5 * GetDifficultyMultiplier(false));

            int planetLevel = 1;
            if (product.Type == Product.ProductType.FOOD)
                planetLevel = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.CommerceLevel);
            else if (product.Type == Product.ProductType.TECNO)
                planetLevel = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.TecnoLevel);
            else //productType == TOOL
                planetLevel = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.ScienceLevel);

            int lvlDiff = planetLevel - product.Level;
            if (lvlDiff <= 0)
                i *= 2;
            else
                i /= 2;
            if (GameManager.ActualSystemOwner.EmpireName == GameManager.PlayerEmpire.EmpireName)
                i /= 2;
            if (GameManager.ActualSystemOwner.ShowRelationWithPlayer().RelationPoints < 0)
                i *= 2;

            if (i <= 0)
                i = 1;
            return i;
        }

        static public List<PlayerPowerUp> GetPowerUpList()
        {
            List<PlayerPowerUp> powerUps = new List<PlayerPowerUp>();
            List<PlayerPowerUp> player = PlayerShip.PowerUps;

            foreach (PlayerPowerUp.PowerUpType put in Enum.GetValues(typeof(PlayerPowerUp.PowerUpType)))
            {
                int level = ReturnLevelTypeForPowerUp(put, actualPlanet.PlanetSettlement);
                PlayerPowerUp p = new PlayerPowerUp(put, level);
                bool present = false;
                foreach(PlayerPowerUp ppu in player)
                    if(ppu.Level == p.Level && ppu.Type == p.Type)
                    {
                        present = true;
                        break;
                    }
                if (!present)
                    powerUps.Add(p);
            }

            return powerUps;
        }

        static public int GetPowerUpPrize(PlayerPowerUp powerUp)
        {
            int i = (int)(powerUp.Level * 512 * Math.Sqrt(powerUp.Level) * GetDifficultyMultiplier(true));
            if (actualPlanet == GameManager.PlayerCapital)
                return i;
            int lvlDiff = LevelManager.ActualLevel(actualPlanet.PlanetSettlement.CommerceLevel) - LevelManager.ActualLevel(GameManager.PlayerCapital.PlanetSettlement.CommerceLevel);
            if (lvlDiff <= 0)
                i /= 2;
            else
                i *= 2;
            if (ReturnLevelTypeForPowerUp(powerUp.Type, actualPlanet.PlanetSettlement) - ReturnLevelTypeForPowerUp(powerUp.Type, GameManager.PlayerCapital.PlanetSettlement) < 0)
                i /= 2;
            if (powerUp.Level > ReturnLevelTypeForPowerUp(powerUp.Type, GameManager.PlayerCapital.PlanetSettlement))
                i *= 4;
            if (GameManager.ActualSystemOwner.EmpireName == GameManager.PlayerEmpire.EmpireName)
                i /= 4;
            if (GameManager.ActualSystemOwner.ShowRelationWithPlayer().RelationPoints < 0)
                i *= 2;
            else
                i /= 2;

            if (i <= 0)
                i = 1;
            return i;
        }

        static int ReturnLevelTypeForPowerUp(PlayerPowerUp.PowerUpType type, Settlement settlement)
        {
            int commerce = LevelManager.ActualLevel(settlement.CommerceLevel);
            int tecno = LevelManager.ActualLevel(settlement.TecnoLevel);
            int science = LevelManager.ActualLevel(settlement.ScienceLevel);

            int level = 1;
            switch (type)
            {
                case PlayerPowerUp.PowerUpType.CARGO:
                case PlayerPowerUp.PowerUpType.SHIELD:
                    level = commerce;
                    break;
                case PlayerPowerUp.PowerUpType.ENGINE:
                case PlayerPowerUp.PowerUpType.TERRAFORMER:
                    level = science;
                    break;
                case PlayerPowerUp.PowerUpType.LASER:
                case PlayerPowerUp.PowerUpType.ROCKET:
                    level = tecno;
                    break;

            }
            return level;
        }

        /// <summary>
        /// Metodo che ritorna il moltiplicatore da assegnare al prezzo a seconda della difficoltà.
        /// </summary>
        /// <param name="multiply">True se il prezzo deve essere moltiplicato (es. a difficile costa di più), 
        ///                         false se deve essere diviso (es. a difficile pagano di meno)</param>
        /// <returns></returns>
        static float GetDifficultyMultiplier(bool multiply)
        {
            float prize = 1f;
            switch(GameParams.gameDifficulty)
            {
                case GameParams.Difficulty.EASY:
                    if (multiply)
                        prize = 0.66f;
                    else
                        prize = 1.33f;
                    break;
                case GameParams.Difficulty.NORMAL:
                    if (multiply)
                        prize = 1.25f;
                    else
                        prize = 0.88f;
                    break;
                case GameParams.Difficulty.HARD:
                    if (multiply)
                        prize = 1.33f;
                    else
                        prize = 0.66f;
                    break;
            }
            return prize;
        }

    }
}
