using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;

namespace GalacticEmpire
{
    static class LoadGameClass
    {
        static GameWindow.GameState state = GameWindow.GameState.GALAXY;
        static public GameWindow.GameState State { get { return state; } }

        /// <summary>
        /// Metodo che carica il file desiderato
        /// </summary>
        /// <param name="filePath">Percorso del file da caricare</param>
        static public void LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            using (XmlReader xr = XmlReader.Create(filePath))
            {
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xr.Name)
                            {
                                case "INFO":
                                    ReadInfo(xr);
                                    break;
                                case "SOLARSYSTEMS":
                                    ReadSolarSystems(xr);
                                    break;
                                case "EMPIRES":
                                    ReadEmpires(xr);
                                    break;
                                case "PLAYEREMPIRE":
                                    while ((xr.Read()) && xr.Name != "EMPIRE") { } // Avanza fino a products
                                    GameManager.PlayerEmpire = ReadEmpire(xr);
                                    break;
                                case "PLAYERSHIP":
                                    ReadPlayer(xr);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Metodo che legge le informazioni del gioco
        /// </summary>
        /// <param name="xr">Istanza Xml Reader</param>
        static void ReadInfo(XmlReader xr)
        {
            while(xr.MoveToNextAttribute())
            {
                switch(xr.Name)
                {
                    case "EMPIRENAME":
                        break;
                    case "SAVETIME":
                        break;
                    case "VERSION":
                        break;
                    case "STATE":
                        if (xr.Value == "SYSTEM")
                            state = GameWindow.GameState.SYSTEM;
                        else
                            state = GameWindow.GameState.GALAXY;
                        break;
                    case "DIFFICULTY":
                        if(xr.Value == "EASY")
                            GameParams.gameDifficulty = GameParams.Difficulty.EASY;
                        else if(xr.Value == "NORMAL")
                            GameParams.gameDifficulty = GameParams.Difficulty.NORMAL;
                        else
                            GameParams.gameDifficulty = GameParams.Difficulty.HARD;
                        break;
                }
            }
        }

        /// <summary>
        /// Metodo che legge i sistemi solari da XML e li inserisce nella lista del GameManager
        /// </summary>
        /// <param name="xr">XML Reader</param>
        static void ReadSolarSystems(XmlReader xr)
        {
            List<SolarSystem> solarSystems = new List<SolarSystem>();
            while (xr.Read() && !(xr.Name == "SOLARSYSTEMS" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.Name == "SYSTEM" && xr.NodeType == XmlNodeType.Element)
                    solarSystems.Add(ReadSystem(xr));
            }
            GameManager.SolarSystems = solarSystems;
        }

        /// <summary>
        /// Metodo che legge il sistema solare da XML
        /// </summary>
        /// <param name="xr">Lettore XML</param>
        /// <returns>Istanza del SistemaSolare</returns>
        static SolarSystem ReadSystem(XmlReader xr)
        {
            List<Planet> planets = new List<Planet>();
            Planet actualPlanet = null;

            //Attributi del sistema
            string name = "", starType = "";
            bool discovered = false, inhabited = false;
            Vector3 position = new Vector3();

            while(xr.MoveToNextAttribute())
            {
                switch(xr.Name)
                {//NAME="Mestu" POSITION="{X:-64 Y:0 Z:-32}" TYPE="Red" DISCOVERED="True" INHABITED="True"
                    case "NAME":
                        name = xr.Value;
                        break;
                    case "POSITION":
                        position = NewVector3FromString(xr.Value);
                        break;
                    case "TYPE":
                        starType = xr.Value;
                        break;
                    case "DISCOVERED":
                        discovered = xr.ReadContentAsBoolean();
                        break;
                    case "INHABITED":
                        inhabited = xr.ReadContentAsBoolean();
                        break;
                }
            }

            while (xr.Read() && !(xr.Name == "SYSTEM" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.NodeType == XmlNodeType.EndElement)
                    continue;
                switch (xr.Name)
                {
                    case "PLANET":
                        actualPlanet = ReadPlanet(xr, position);
                        planets.Add(actualPlanet);
                        break;
                    case "SETTLEMENT":
                        actualPlanet.CreateSettlement(ReadSettlement(xr));
                        break;
                    case "PRODUCTS":
                        actualPlanet.PlanetSettlement.SetProducts(ReadProducts(xr));
                        break;
                }
            }

            SolarSystem ss = new SolarSystem(name, position, starType, discovered, inhabited, planets);
            return ss;
        }
        
        /// <summary>
        /// Metodo che legge il pianeta da XML
        /// </summary>
        /// <param name="xr">Istanza XML reader</param>
        /// <param name="StarPosition">Posizione della stella cui si riferisce</param>
        /// <returns></returns>
        static Planet ReadPlanet(XmlReader xr, Vector3 StarPosition)
        {
            //PLANET NAME="Ghipisu" POSITION="{X:173,6179 Y:0 Z:366,2257}" TYPE="Gas6" HABITABLE="False" TERRASCORE="0" RESOURCES="2504679350" 
            string name = "", type = "";
            bool habitable = false;
            int terrascore = 0;
            uint resources = 0;
            Vector3 position = new Vector3();
            while (xr.MoveToNextAttribute())
            {
                switch(xr.Name)
                {
                    case "NAME":
                        name = xr.Value;
                        break;
                    case "POSITION":
                        position = NewVector3FromString(xr.Value);
                        break;
                    case "TYPE":
                        type = xr.Value;
                        break;
                    case "HABITABLE":
                        habitable = xr.ReadContentAsBoolean();
                        break;
                    case "TERRASCORE":
                        terrascore = xr.ReadContentAsInt();
                        break;
                    case "RESOURCES":
                        resources = Convert.ToUInt32(xr.Value);
                        break;
                }
            }
            return new Planet(name, position, StarPosition, type, habitable, terrascore, resources);
        }

        /// <summary>
        /// Metodo che legge l'insediamento da XML
        /// </summary>
        /// <param name="xr">Istanza del lettore XML</param>
        /// <returns>Istanza dell'insediamento</returns>
        static Settlement ReadSettlement(XmlReader xr)
        {
            //< SETTLEMENT TYPE = "CAPITAL" MONEY = "49979" INHABITANTS = "582913" INHARATE = "1,0004" SCIENCE = "3,200001" SCIENCERATE = "0,2"
            //COMMERCE = "9,800001" COMMERCERATE = "0,8" TECNO = "2,1" TECNORATE = "0,1" OFFENSIVE = "190" DEFENSIVE = "8584" >
            Settlement.SettlementType type = Settlement.SettlementType.INHABITED;
            int money = 0, inhabitants = 0, defensive = 0, offensive = 0;
            float inhaRate = 0, science = 0, scienceRate = 0, commerce = 0, commerceRate = 0, tecno = 0, tecnoRate = 0;

            while (xr.MoveToNextAttribute())
            {
                switch (xr.Name)
                {
                    case "TYPE":
                        type = NewSettlementTypeFromString(xr.Value);
                        break;
                    case "MONEY":
                        money = xr.ReadContentAsInt();
                        break;
                    case "INHABITANTS":
                        inhabitants = xr.ReadContentAsInt();
                        break;
                    case "INHARATE":
                        inhaRate = Convert.ToSingle(xr.Value);
                        break;
                    case "SCIENCE":
                        science = Convert.ToSingle(xr.Value);
                        break;
                    case "SCIENCERATE":
                        scienceRate = Convert.ToSingle(xr.Value);
                        break;
                    case "COMMERCE":
                        commerce = Convert.ToSingle(xr.Value);
                        break;
                    case "COMMERCERATE":
                        commerceRate = Convert.ToSingle(xr.Value);
                        break;
                    case "TECNO":
                        tecno = Convert.ToSingle(xr.Value);
                        break;
                    case "TECNORATE":
                        tecnoRate = Convert.ToSingle(xr.Value);
                        break;
                    case "OFFENSIVE":
                        offensive = xr.ReadContentAsInt();
                        break;
                    case "DEFENSIVE":
                        defensive = xr.ReadContentAsInt();
                        break;
                }
            }
            return new Settlement(type, money, inhabitants, defensive, offensive, inhaRate,
                                  science, scienceRate, commerce, commerceRate, tecno, tecnoRate);
        }

        /// <summary>
        /// Metodo che legge i prodotti da XML
        /// </summary>
        /// <param name="xr">Istanza di XML Reader</param>
        /// <returns>Lista di prodotti</returns>
        static List<Product> ReadProducts(XmlReader xr)
        {
            //< PRODUCTS >
            //  < PRODUCT TYPE = "FOOD" QUANTITY = "28" LEVEL = "1" />
            //  < PRODUCT TYPE = "TECNO" QUANTITY = "14" LEVEL = "1" />
            //  < PRODUCT TYPE = "TOOL" QUANTITY = "14" LEVEL = "1" />
            //</ PRODUCTS >
            List<Product> products = new List<Product>();
            if (xr.IsEmptyElement)
                return products;
            while (xr.Read() && !(xr.Name == "PRODUCTS" && xr.NodeType == XmlNodeType.EndElement))
            {
                if(xr.Name == "PRODUCT")
                {
                    int qt = 0, level = 0;
                    Product.ProductType type = Product.ProductType.FOOD;
                    while(xr.MoveToNextAttribute())
                    {
                        switch(xr.Name)
                        {
                            case "TYPE":
                                if (xr.Value == "TECNO")
                                    type = Product.ProductType.TECNO;
                                else if (xr.Value == "TOOL")
                                    type = Product.ProductType.TOOL;
                                break;
                            case "QUANTITY":
                                qt = Convert.ToInt32(xr.Value);
                                break;
                            case "LEVEL":
                                level = Convert.ToInt32(xr.Value);
                                break;
                        }
                    }
                    products.Add(new Product(type, qt, level));
                }
            }
            return products;
        }

        /// <summary>
        /// Metodo che legge gli imperi da XML e li salva nella lista di GameManager
        /// </summary>
        /// <param name="xr">Istanza XML Reader</param>
        static void ReadEmpires(XmlReader xr)
        {
            List<Empire> empires = new List<Empire>();
            while (xr.Read() && !(xr.Name == "EMPIRES" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.Name == "EMPIRE" && xr.NodeType == XmlNodeType.Element)
                    empires.Add(ReadEmpire(xr));
            }
            GameManager.Empires = empires;
        }

        static Empire ReadEmpire(XmlReader xr)
        {
            List<SolarSystem> ownedSystems = new List<SolarSystem>();
            List<SolarSystem> knownSystems = new List<SolarSystem>();
            List<Ship> fleet = new List<Ship>();
            List<Relation> relations = new List<Relation>();

            string name = "";
            Religion.ReligionType religion = Religion.ReligionType.ATEO;
            while (xr.MoveToNextAttribute())
            {
                if (xr.Name == "NAME")
                    name = xr.Value;
                else if (xr.Name == "RELIGION")
                    religion = ReligionFromString(xr.Value);
            }

            while (xr.Read() && !(xr.Name == "EMPIRE" && xr.NodeType == XmlNodeType.EndElement))
            {
                switch(xr.Name)
                {
                    case "OWNEDSYSTEMS":
                        ownedSystems = ReadKnownSystems(xr, "OWNEDSYSTEMS");
                        break;
                    case "KNOWNSYSTEMS":
                        knownSystems = ReadKnownSystems(xr, "KNOWNSYSTEMS");
                        break;
                    case "FLEET":
                        fleet = ReadFleet(xr);
                        break;
                    case "RELATIONS":
                        relations = ReadRelations(xr);
                        break;
                }
            }
            Planet capital = null;
            foreach(SolarSystem ss in ownedSystems)
                foreach(Planet p in ss.Planets)
                    if(p.PlanetSettlement != null && p.PlanetSettlement.Type == Settlement.SettlementType.CAPITAL)
                    {
                        capital = p;
                        break;
                    }
            return new Empire(name, religion, knownSystems, ownedSystems, fleet, relations, capital);
        }

        /// <summary>
        /// Metodo che legge i sistemi solari conosciuti o posseduti dall'impero a partire da nome e posizione
        /// </summary>
        /// <param name="xr">Istanza XML Reader</param>
        /// <param name="typeSys">Tipo di sistemi, se posseduti o conosciuti</param>
        /// <returns>Lista di sistemi solari</returns>
        static List<SolarSystem> ReadKnownSystems(XmlReader xr, string typeSys)
        {//<SYSTEM NAME="Mestu" POSITION="{X:-64 Y:0 Z:-32}" />
            List<SolarSystem> sys = new List<SolarSystem>();
            if (xr.IsEmptyElement)
                return sys;
            //Finché non si arriva alla fine dell'elemento
            while (xr.Read() && !(xr.Name == typeSys && xr.NodeType == XmlNodeType.EndElement))
            {
                if(xr.Name == "SYSTEM")
                {//Ottieni nome e posizione del sistema solare
                    Vector3 position = new Vector3();
                    string name = "";

                    while (xr.MoveToNextAttribute())
                    {
                        if (xr.Name == "NAME")
                            name = xr.Value;
                        else if (xr.Name == "POSITION")
                            position = NewVector3FromString(xr.Value);
                    }
                    //Controlla se tra tutti i sistemi solari esiste e mettilo nella lista
                    foreach(SolarSystem ss in GameManager.SolarSystems)
                    {
                        if (ss.Name == name && ss.SystemPosition == position)
                        {//Se lo hai trovato aggiungilo alla lista ed esci dal foreach
                            sys.Add(ss);
                            break;
                        }
                    }
                }
            }
            return sys;
        }

        /// <summary>
        /// Metodo che legge la flotta di un impero
        /// </summary>
        /// <param name="xr">Istanza XML Reader</param>
        /// <returns>Flotta dell'impero</returns>
        static List<Ship> ReadFleet(XmlReader xr)
        {
            //<SHIP LIFE="709" MAXLIFE="709" POSITION="{X:-7648 Y:0 Z:7904}" TARGETPOSITION="{X:-7104 Y:0 Z:7680}" SPEED="11" MAXCARGO="531">
            List<Ship> fleet = new List<Ship>();

            if (xr.IsEmptyElement)
                return fleet;

            while (xr.Read() && !(xr.Name == "FLEET" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.Name == "SHIP" && xr.NodeType == XmlNodeType.Element)
                {
					int life = 0, maxLife = 0, speed = 0, maxCargo = 0, offensive = 0;
					Vector3 position = new Vector3(), target = new Vector3();
					List<Product> products = new List<Product>();
					bool isCargo = true;
                    while(xr.MoveToNextAttribute())
                    {
                        switch(xr.Name)
                        {
                            case "LIFE":
                                life = xr.ReadContentAsInt();
                                break;
                            case "MAXLIFE":
                                maxLife = xr.ReadContentAsInt();
                                break;
                            case "POSITION":
                                position = NewVector3FromString(xr.Value);
                                break;
                            case "TARGETPOSITION":
                                target = NewVector3FromString(xr.Value);
                                break;
                            case "SPEED":
                                speed = xr.ReadContentAsInt();
                                break;
                            case "MAXCARGO":
                                maxCargo = xr.ReadContentAsInt();
                                isCargo = true;
                                break;
                            case "OFFENSIVE":
                                offensive = xr.ReadContentAsInt();
                                isCargo = false;
                                break;
                        }
                    }

                    if (isCargo)
                    {
                        while((xr.Read()) && xr.Name != "PRODUCTS") { } // Avanza fino a products
                        ReadProducts(xr);
                        fleet.Add(new CommercialShip(position, target, products, life, speed, maxCargo, maxLife));
                    }
                    else
                        fleet.Add(new WarShip(position, target, life, speed, offensive, maxLife));
                }
            }
            return fleet;
        }

        /// <summary>
        /// Metodo che legge da XML le relazioni dell'impero
        /// </summary>
        /// <param name="xr">Istanza XML Reader</param>
        /// <returns>Lista delle relazioni</returns>
        static List<Relation> ReadRelations(XmlReader xr)
        {
            List<Relation> relations = new List<Relation>();
            if (xr.IsEmptyElement)
                return relations;
            while (xr.Read() && !(xr.Name == "RELATIONS" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.Name == "RELATION" && xr.NodeType == XmlNodeType.Element)
                {
					int points = 0;
					string empireName = "";
					List<Event> events = new List<Event>();
                    while(xr.MoveToNextAttribute())
                    {
                        if (xr.Name == "POINTS")
                            points = xr.ReadContentAsInt();
                        else if (xr.Name == "NAME")
                            empireName = xr.Value;
                    }
                    events = ReadEvents(xr);
                    relations.Add(new Relation(empireName, events));
                }
            }
            return relations;
        }

        /// <summary>
        /// Metodo che legge gli eventi di una relazione
        /// </summary>
        /// <param name="xr"></param>
        /// <returns></returns>
        static List<Event> ReadEvents(XmlReader xr)
        {
            List<Event> events = new List<Event>();
            if (xr.IsEmptyElement)
                return events;
            while (xr.Read() && !(xr.Name == "EVENTS" && xr.NodeType == XmlNodeType.EndElement))
            {
                if(xr.Name == "EVENT")
                {
                    Event.EventType type = Event.EventType.NEUTRAL;
                    Event.EventMotivation motivation = Event.EventMotivation.GIFT;
                    int points = 0;

                    while (xr.MoveToNextAttribute())
                    {
                        switch(xr.Name)
                        {
                            case "MOTIVATION":
                                motivation = MotivationFromString(xr.Value);
                                break;
                            case "TYPE":
                                if (xr.Value == "POSITIVE")
                                    type = Event.EventType.POSITIVE;
                                else if (xr.Value == "NEGATIVE")
                                    type = Event.EventType.NEGATIVE;
                                break;
                            case "POINTS":
                                points = xr.ReadContentAsInt();
                                break;
                        }
                    }
                    events.Add(new Event(type, motivation, points));
                }
            }
            return events;
        }

        static void ReadPlayer(XmlReader xr)
        {   //LIFE="1500" MAXLIFE="1500" ENERGY="500" MAXENERGY="500" POSITION="{X:0 Y:50 Z:0}" TARGETPOSITION="{X:0 Y:50 Z:0}"
            //SPEED ="10" DEFENSIVE="0" OFFENSIVE="0" MONEY="100000000" MAXCARGO="0" RADIUS="2000"
            int life = 0, maxLife = 0, speed = 0, defensive = 0, offensive = 0, maxCargo = 0, radius = 0, money = 0;
            float energy = 0, maxEnergy = 0;
            Vector3 position = new Vector3(), target = new Vector3();
            List<Product> products = new List<Product>();
            List<PlayerPowerUp> powerUps = new List<PlayerPowerUp>();

            while (xr.MoveToNextAttribute())
            {
                switch(xr.Name)
                {
                    case "LIFE":
                        life = xr.ReadContentAsInt();
                        break;
                    case "MAXLIFE":
                        maxLife = xr.ReadContentAsInt();
                        break;
                    case "OFFENSIVE":
                        offensive = xr.ReadContentAsInt();
                        break;
                    case "DEFENSIVE":
                        defensive = xr.ReadContentAsInt();
                        break;
                    case "MAXCARGO":
                        maxCargo = xr.ReadContentAsInt();
                        break;
                    case "SPEED":
                        speed = xr.ReadContentAsInt();
                        break;
                    case "RADIUS":
                        radius = xr.ReadContentAsInt();
                        break;
                    case "MONEY":
                        money = xr.ReadContentAsInt();
                        break;
                    case "ENERGY":
                        energy = Convert.ToSingle(xr.Value);
                        break;
                    case "MAXENERGY":
                        maxEnergy = Convert.ToSingle(xr.Value);
                        break;
                    case "POSITION":
                        position = NewVector3FromString(xr.Value);
                        break;
                    case "TARGETPOSITION":
                        target = NewVector3FromString(xr.Value);
                        break;
                }
            }
            while (xr.Read() && !(xr.Name == "PLAYERSHIP" && xr.NodeType == XmlNodeType.EndElement))
            {
                if (xr.Name == "PRODUCTS" && xr.NodeType == XmlNodeType.Element)
                    products = ReadProducts(xr);
                else if (xr.Name == "POWERUPS" && xr.NodeType == XmlNodeType.Element)
                    powerUps = ReadPlayerPowerUps(xr);
            }
            PlayerShip.Initialize(position, target, life, maxLife, energy, maxEnergy, speed, radius, money, maxCargo, products, powerUps);
        }

        static List<PlayerPowerUp> ReadPlayerPowerUps(XmlReader xr)
        {
            return new List<PlayerPowerUp>();
        }

        /// <summary>
        /// Metodo che crea un Vector3 a partire da una stringa
        /// </summary>
        /// <param name="str">Stringa in cui sono scritti i parametri del vettore</param>
        /// <returns>Vettore originato dalla stringa</returns>
        static Vector3 NewVector3FromString(string str)
        {
            Vector3 result = new Vector3();
            ///{X:-64 Y:0 Z:-32}
            string[] charToReplace = { "{", "}", "X", "Y", "Z", ":" };
            foreach(string s in charToReplace)
                str = str.Replace(s, "");
            ///-64 0 -32
            string[] v = str.Split(' ');
            ///[ -64 ; 0 ; -32 ]
            try //Nel caso in cui vada male ritorna il vettore nullo
            {
                result.X = Convert.ToSingle(v[0]);
                result.Y = Convert.ToSingle(v[1]);
                result.Z = Convert.ToSingle(v[2]);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Metodo che restituisce il tipo di colonia a partire da una stringa.
        /// </summary>
        /// <param name="str">Stringa che contiene il tipo di colonia</param>
        /// <returns>Tipo di colonia ottenuto</returns>
        static Settlement.SettlementType NewSettlementTypeFromString(string str)
        {
            switch(str)
            {
                case "CAPITAL":
                    return Settlement.SettlementType.CAPITAL;
                case "COMMUNITY":
                    return Settlement.SettlementType.COMMUNITY;
                case "MILITARY":
                    return Settlement.SettlementType.MILITARY;
                case "INHABITED":
                default:
                    return Settlement.SettlementType.INHABITED;
            }
        }

        /// <summary>
        /// Metodo che restituisce il tipo di religione a partire da una stringa.
        /// </summary>
        /// <param name="str">Stringa che contiene il tipo di colonia</param>
        /// <returns>Tipo di religione ottenuto</returns>
        static Religion.ReligionType ReligionFromString(string str)
        {
            switch (str)
            {
                case "ATEO":
                    return Religion.ReligionType.ATEO;
                case "ATOM":
                    return Religion.ReligionType.ATOM;
                case "BLESS":
                    return Religion.ReligionType.BLESS;
                case "CURSER":
                default:
                    return Religion.ReligionType.CURSER;
            }
        }

        /// <summary>
        /// Metodo che restituisce il tipo di motivazione di un evento a partire da una stringa.
        /// </summary>
        /// <param name="str">Stringa che contiene il tipo di motivazione</param>
        /// <returns>Tipo di motivazione ottenuto</returns>
        static Event.EventMotivation MotivationFromString(string str)
        {
            switch (str)
            {
                case "COMMERCE":
                    return Event.EventMotivation.COMMERCE;
                case "GIFT":
                    return Event.EventMotivation.GIFT;
                case "RELIGION":
                    return Event.EventMotivation.RELIGION;
                case "SCIENCE":
                    return Event.EventMotivation.SCIENCE;
                case "TECNO":
                    return Event.EventMotivation.TECNO;
                case "WAR":
                default:
                    return Event.EventMotivation.WAR;
            }
        }
    }
}
