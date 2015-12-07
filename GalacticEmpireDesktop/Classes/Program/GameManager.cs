using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GalacticEmpire
{
    static class GameManager
    {
        static List<SolarSystem> solarSystems;
        static public List<SolarSystem> SolarSystems { get { return solarSystems; } }

        static Empire playerEmpire;
        static public Empire PlayerEmpire { get { return playerEmpire; } }

        static Planet playerCapital;
        static public Planet PlayerCapital { get { return playerCapital; } }

        static List<Empire> empires;
        public static List<Empire> Empires { get { return empires; } }

        static SolarSystem actualSystem;
        static public SolarSystem ActualSystem { get { return actualSystem; } }

        static Empire actualSystemOwner;
        static public Empire ActualSystemOwner { get { return actualSystemOwner; } }

        static int lastSecondUpdate = 0;

        static public void NewGame()
        {
            solarSystems = new List<SolarSystem>();
            solarSystems.Clear();

            empires = new List<Empire>();
            empires.Clear();

            int systems = GameParams.starNumber;
            int numEmpires = GameParams.empireNumber;

            List<Vector3> vect = new List<Vector3>();
            for (int i = -1 * systems / 2; i < systems / 2; i++)
                for (int j = -1 * systems / 2; j < systems / 2; j++)
                    vect.Add(new Vector3(i * 32, 0, j * 32));

            vect.Remove(new Vector3(0, 0, 0));
            Random rnd = new Random();
            for (int i = 0; i < systems; i++)
            {
                int index = rnd.Next(0, vect.Count);
                Vector3 pos = vect[index];
                SolarSystem solar = new SolarSystem(pos);
                solarSystems.Add(solar);
                vect.RemoveAt(index);
            }
            vect.Clear();
            foreach (SolarSystem ss in solarSystems)
                ss.CreatePlanets();

            CreateEmpires();
            //Crea il sistema solare di partenza per il giocatore
            SolarSystem startingSystem = new SolarSystem(new Vector3(0, 0, 0));
            startingSystem.IsDiscovered = true;
            startingSystem.CreatePlanets(true);
            if (GameParams.name == null)
                GameParams.name = NameGenerator.GetName();
            playerEmpire = new Empire(startingSystem, GameParams.religionType, GameParams.name);
            solarSystems.Add(startingSystem);
            foreach (Planet p in startingSystem.Planets)
                if (p.PlanetSettlement != null)
                    playerCapital = p;

            PlayerShip.Initialize(new Vector3(0, 50, 0), 1500, 500);
            
        }

        static private void CreateEmpires()
        {
            Random rnd = new Random();
            int empireCount = 0;
            foreach(SolarSystem ss in solarSystems)
            {
                foreach(Planet p in ss.Planets)
                {
                    if(p.Terrascore > 20 && empireCount < GameParams.empireNumber)
                    {
                        Empire empire = new Empire(ss);
                        int money = rnd.Next(10000, 100000);
                        int inhabitants = rnd.Next(10000, 1000000);

                        int minD = 300; int minO = 10;
                        if (empire.EmpireReligion == Religion.ReligionType.ATOM || empire.EmpireReligion == Religion.ReligionType.BLESS)
                            minD = 1000;
                        else if (empire.EmpireReligion == Religion.ReligionType.CURSER)
                            minO = 100;
                        int defense = rnd.Next(minD, 10000);
                        int offense = rnd.Next(minO, 200);
                        Settlement settlement = new Settlement(Settlement.SettlementType.CAPITAL, money, inhabitants, defense, offense, empire.EmpireReligion);
                        p.CreateSettlement(settlement);

                        empires.Add(empire);
                        empireCount++;
                        break;
                    }
                }
            }
        }
        
        static public void Update(GameTime gameTime)
        {
            int now = gameTime.TotalGameTime.Seconds;
            PlayerShip.Update();
            foreach (SolarSystem ss in solarSystems)
            {
                ss.UpdatePosition();
                if (ss.SystemPosition.X == PlayerShip.Position.X && ss.SystemPosition.Z == PlayerShip.Position.Z)
                {
                    actualSystem = ss;
                    SystemOwner(); // ottieni l'impero proprietario del sistema
                }
                if (now != lastSecondUpdate && now % 5 == 0)
                    ss.UpdateSystem();
            }

            if (now != lastSecondUpdate && now % 1 == 0)
            {
                foreach (Empire e in empires)
                    e.Update();
            }

            lastSecondUpdate = now;
        }

        /// <summary>
        /// Metodo che controlla il proprietario del sistema su cui si trova attualmente la nave
        /// </summary>
        static public void SystemOwner()
        {
            actualSystemOwner = SystemOwner(actualSystem);
        }

        /// <summary>
        /// Metodo che controlla il proprietario del sistema specificato
        /// </summary>
        /// <param name="solarSys">Sistema solare</param>
        /// <returns>Impero proprietario</returns>
        static public Empire SystemOwner(SolarSystem solarSys)
        {
            Empire Owner = null;
            foreach (SolarSystem ss in playerEmpire.OwnedSystems)
                if (ss == solarSys)
                    Owner = playerEmpire;
            if (Owner == null)
            {
                foreach (Empire e in empires)
                {
                    foreach (SolarSystem ss in e.OwnedSystems)
                        if (ss == solarSys)
                            Owner = e;
                }
            }
            return Owner;
        }
    }
}
