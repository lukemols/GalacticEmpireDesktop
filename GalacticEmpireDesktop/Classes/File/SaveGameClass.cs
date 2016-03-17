using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace GalacticEmpire
{
    /// <summary>
    /// Classe che si occupa del salvataggio del gioco
    /// </summary>
    static class SaveGameClass
    {
        public static void SaveGame(string fileName = "SavedGame")
        {
            //Ottieni la cartella Documenti
            var pathWithEnv = @"%USERPROFILE%\Documents";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            //Crea la cartella Galactic Empire e all'interno quella specifica del gioco
            filePath += @"\Galactic Empire\" + GameManager.PlayerEmpire.EmpireName;
            Directory.CreateDirectory(filePath);
            filePath += @"\" + fileName + ".ges";

            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(filePath, ws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("GAME");
                //Scrivi informazioni file
                SaveInfo(xw);
                //Scrivi i sistemi solari
                SaveSystems(xw);
                //Scrivi gli imperi e l'impero del giocatore
                SaveEmpires(xw);

                xw.WriteStartElement("PLAYEREMPIRE");
                SaveEmpire(xw, GameManager.PlayerEmpire);
                xw.WriteEndElement(); //Di PLAYEREMPIRE
                //Scrivi la nave del giocatore
                SavePlayer(xw);
                

                xw.WriteEndElement(); //Di GAME
                xw.WriteEndDocument();//Del documento
            }

            using (StreamWriter sw = new StreamWriter("LastSave"))
            {
                sw.WriteLine(filePath);
                sw.Close();
            }
        }

        static void SaveInfo(XmlWriter xw)
        {
            xw.WriteStartElement("INFO");
            xw.WriteAttributeString("EMPIRENAME", GameManager.PlayerEmpire.EmpireName);
            xw.WriteAttributeString("SAVETIME", DateTime.Now.ToString());
            xw.WriteAttributeString("VERSION", "1.0");
            xw.WriteAttributeString("STATE", GameWindow.ActualState.ToString());
            xw.WriteAttributeString("DIFFICULTY", GameParams.gameDifficulty.ToString());
            xw.WriteEndElement(); //Di INFO
        }

        static void SaveSystems(XmlWriter xw)
        {
            //Scrivi informazioni dei sistemi solari
            xw.WriteStartElement("SOLARSYSTEMS");
            foreach (SolarSystem ss in GameManager.SolarSystems)
            {
                xw.WriteStartElement("SYSTEM");
                xw.WriteAttributeString("NAME", ss.Name);
                xw.WriteAttributeString("POSITION", ss.SystemPosition.ToString());
                xw.WriteAttributeString("TYPE", ss.StarType.ToString());
                xw.WriteAttributeString("DISCOVERED", ss.IsDiscovered.ToString().ToLowerInvariant());
                xw.WriteAttributeString("INHABITED", ss.IsInhabited.ToString().ToLowerInvariant());

                // Scrivi ora i pianeti del sistema
                SavePlanets(xw, ss.Planets);

                xw.WriteEndElement();//Di SYSTEM
            }
            xw.WriteEndElement();//Di SOLARSYSTEMS
        }

        static void SavePlanets(XmlWriter xw, List<Planet> Planets)
        {
            xw.WriteStartElement("PLANETS");
            foreach (Planet p in Planets)
            {
                xw.WriteStartElement("PLANET");
                xw.WriteAttributeString("NAME", p.Name);
                xw.WriteAttributeString("POSITION", p.PlanetPosition.ToString());
                xw.WriteAttributeString("TYPE", p.PlanetType.ToString());
                xw.WriteAttributeString("HABITABLE", p.IsHabitable.ToString().ToLowerInvariant());
                xw.WriteAttributeString("TERRASCORE", p.Terrascore.ToString());
                xw.WriteAttributeString("RESOURCES", p.Resources.ToString());

                //Se c'è un insediamento inseriscilo
                Settlement s = p.PlanetSettlement;
                if (s != null)
                {
                    SaveSettlement(xw, s);
                }

                xw.WriteEndElement();//Di PLANET

            }
            xw.WriteEndElement();//Di PLANETS
        }

        static void SaveSettlement(XmlWriter xw, Settlement s)
        {
            xw.WriteStartElement("SETTLEMENT");

            xw.WriteAttributeString("TYPE", s.Type.ToString());
            xw.WriteAttributeString("MONEY", s.Money.ToString());
            xw.WriteAttributeString("INHABITANTS", s.Inhabitants.ToString());
            xw.WriteAttributeString("INHARATE", s.InhaGrowthRate.ToString());
            xw.WriteAttributeString("SCIENCE", s.ScienceLevel.ToString());
            xw.WriteAttributeString("SCIENCERATE", s.ScienceGrowthRate.ToString());
            xw.WriteAttributeString("COMMERCE", s.CommerceLevel.ToString());
            xw.WriteAttributeString("COMMERCERATE", s.CommerceGrowthRate.ToString());
            xw.WriteAttributeString("TECNO", s.TecnoLevel.ToString());
            xw.WriteAttributeString("TECNORATE", s.TecnoGrowthRate.ToString());

            xw.WriteAttributeString("OFFENSIVE", s.OffensiveArmy.ToString());
            xw.WriteAttributeString("DEFENSIVE", s.DefensiveArmy.ToString());

            //Inserisci i prodotti dell'insediamento
            SaveProducts(xw, s.Products);
            xw.WriteEndElement();//Di SETTLEMENT
        }

        static void SaveEmpires(XmlWriter xw)
        {
            //Scrivi informazioni degli imperi
            xw.WriteStartElement("EMPIRES");
            foreach (Empire e in GameManager.Empires)
            {
                SaveEmpire(xw, e);
            }
            xw.WriteEndElement();//Di EMPIRES
        }

        static void SaveEmpire(XmlWriter xw, Empire e)
        {
            xw.WriteStartElement("EMPIRE");
            xw.WriteAttributeString("NAME", e.EmpireName);
            xw.WriteAttributeString("RELIGION", e.EmpireReligion.ToString());

            // Scrivi ora i sistemi posseduti e conosciuti
            SaveKnownSystem(xw, "OWNEDSYSTEMS", e.OwnedSystems);
            SaveKnownSystem(xw, "KNOWNSYSTEMS", e.KnownSystems);

            // Relazioni con altri imperi
            SaveRelation(xw, e.EmpireRelations);

            // Flotta
            SaveFleet(xw, e.Fleet);

            xw.WriteEndElement();//Di EMPIRE
        }

        static void SaveKnownSystem(XmlWriter xw, string type, List<SolarSystem> systems)
        {
            xw.WriteStartElement(type);
            foreach (SolarSystem ss in systems)
            {
                xw.WriteStartElement("SYSTEM");
                xw.WriteAttributeString("NAME", ss.Name);
                xw.WriteAttributeString("POSITION", ss.SystemPosition.ToString());
                xw.WriteEndElement();//Di SYSTEM
            }
            xw.WriteEndElement();//Di OWNEDSYSTEMS
        }

        static void SaveRelation(XmlWriter xw, List<Relation> relations)
        {
            xw.WriteStartElement("RELATIONS");
            foreach (Relation r in relations)
            {
                xw.WriteStartElement("RELATION");

                xw.WriteAttributeString("NAME", r.EmpireName);
                xw.WriteAttributeString("POINTS", r.RelationPoints.ToString());

                //Salva gli eventi
                xw.WriteStartElement("EVENTS");
                foreach (Event ev in r.RelationEvents)
                {
                    xw.WriteStartElement("EVENT");
                    xw.WriteAttributeString("TYPE", ev.Type.ToString());
                    xw.WriteAttributeString("POINTS", ev.EventPoints.ToString());
                    xw.WriteAttributeString("MOTIVATION", ev.Motivation.ToString());

                    xw.WriteEndElement();//Di EVENT
                }
                xw.WriteEndElement();//Di EVENTS

                xw.WriteEndElement();//Di RELATION
            }
            xw.WriteEndElement();//Di RELATIONS
        }

        static void SaveFleet(XmlWriter xw, List<Ship> fleet)
        {
            //Scrivi la flotta
            xw.WriteStartElement("FLEET");
            foreach (Ship s in fleet)
            {
                xw.WriteStartElement("SHIP");
                xw.WriteAttributeString("LIFE", s.Life.ToString());
                xw.WriteAttributeString("MAXLIFE", s.MaxLife.ToString());
                xw.WriteAttributeString("POSITION", s.Position.ToString());
                xw.WriteAttributeString("TARGETPOSITION", s.TargetPosition.ToString());
                xw.WriteAttributeString("SPEED", s.Speed.ToString());
                if (s is WarShip)
                {
                    WarShip w = s as WarShip;

                    xw.WriteAttributeString("OFFENSIVE", w.OffensivePower.ToString());
                }
                else
                {
                    CommercialShip c = s as CommercialShip;

                    xw.WriteAttributeString("MAXCARGO", c.MaxCargo.ToString());
                    SaveProducts(xw, c.Products);
                }

                xw.WriteEndElement();//Di WAR/COMMERCE SHIP
            }
            xw.WriteEndElement();//Di FLEET
        }

        static void SavePlayer(XmlWriter xw)
        {
            xw.WriteStartElement("PLAYERSHIP");

            xw.WriteAttributeString("LIFE", PlayerShip.Life.ToString());
            xw.WriteAttributeString("MAXLIFE", PlayerShip.MaxLife.ToString());
            xw.WriteAttributeString("ENERGY", PlayerShip.Energy.ToString());
            xw.WriteAttributeString("MAXENERGY", PlayerShip.MaxEnergy.ToString());
            xw.WriteAttributeString("POSITION", PlayerShip.Position.ToString());
            xw.WriteAttributeString("TARGETPOSITION", PlayerShip.TargetPosition.ToString());
            xw.WriteAttributeString("SPEED", PlayerShip.Speed.ToString());
            xw.WriteAttributeString("DEFENSIVE", PlayerShip.DefensiveArmy.ToString());
            xw.WriteAttributeString("OFFENSIVE", PlayerShip.OffensiveArmy.ToString());
            xw.WriteAttributeString("MONEY", PlayerShip.Money.ToString());
            xw.WriteAttributeString("MAXCARGO", PlayerShip.MaxCargo.ToString());
            xw.WriteAttributeString("RADIUS", PlayerShip.Radius.ToString());

            //Inserisci i prodotti della nave se ce ne sono
            if (PlayerShip.Products.Count > 0)
                SaveProducts(xw, PlayerShip.Products);

            //Inserisci i power up (potenziamenti) della nave se ce ne sono
            if (PlayerShip.PowerUps.Count > 0)
                SavePowerUps(xw);

            xw.WriteEndElement();//Di PLAYERSHIP
        }

        static void SaveProducts(XmlWriter xw, List<Product> Products)
        {
            xw.WriteStartElement("PRODUCTS");
            foreach (Product pr in Products)
            {
                xw.WriteStartElement("PRODUCT");

                xw.WriteAttributeString("TYPE", pr.Type.ToString());
                xw.WriteAttributeString("QUANTITY", pr.Quantity.ToString());
                xw.WriteAttributeString("LEVEL", pr.Level.ToString());

                xw.WriteEndElement();//Di PRODUCT
            }
            xw.WriteEndElement();//Di PRODUCTS
        }

        static void SavePowerUps(XmlWriter xw)
        {
            xw.WriteStartElement("POWERUPS");
            foreach (PlayerPowerUp pr in PlayerShip.PowerUps)
            {
                xw.WriteStartElement("POWERUP");

                xw.WriteAttributeString("TYPE", pr.Type.ToString());
                xw.WriteAttributeString("LEVEL", pr.Level.ToString());

                xw.WriteEndElement();//Di POWERUP
            }
            xw.WriteEndElement();//Di POWERUPS
        }

    }
}
