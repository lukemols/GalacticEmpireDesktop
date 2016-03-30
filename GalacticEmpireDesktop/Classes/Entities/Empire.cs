using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GalacticEmpire
{
    class Empire
    {
        string empireName;
        public string EmpireName { get { return empireName; } }

        Planet capitalPlanet;

        List<SolarSystem> ownedSystems;
        public List<SolarSystem> OwnedSystems { get { return ownedSystems; } }

        List<SolarSystem> knownSystems;
        public List<SolarSystem> KnownSystems { get { return knownSystems; } }

        List<Relation> empireRelations;
        public List<Relation> EmpireRelations { get { return empireRelations; } }

        Religion.ReligionType empireReligion;
        public Religion.ReligionType EmpireReligion { get { return empireReligion; } }

        List<Ship> fleet;
        public List<Ship> Fleet { get { return fleet; } }

        static Random rnd;

        int lastSecondUpdate = 0;
        int missingSecondsToUpdate = 0;

        public Empire(SolarSystem startingSystem)
        {
            knownSystems = new List<SolarSystem>();
            ownedSystems = new List<SolarSystem>();
            ownedSystems.Add(startingSystem);
            fleet = new List<Ship>();
            empireRelations = new List<Relation>();
            empireReligion = Religion.GetRandomReligion();
            empireName = NameGenerator.GetName(4);
            if(rnd == null) 
               rnd = new Random();
            missingSecondsToUpdate = rnd.Next(1, 60);
        }

        public Empire(SolarSystem startingSystem, Religion.ReligionType religion, string name)
        { 
            knownSystems = new List<SolarSystem>();
            ownedSystems = new List<SolarSystem>();
            ownedSystems.Add(startingSystem);
            fleet = new List<Ship>();
            empireRelations = new List<Relation>();
            empireReligion = religion;
            empireName = name;
            if(rnd == null)
               rnd = new Random();
            missingSecondsToUpdate = rnd.Next(1, 60);
        }

        public Empire(string name, Religion.ReligionType religion, List<SolarSystem> knownSys, List<SolarSystem> ownedSys, List<Ship> fleet,
            List<Relation> relations, Planet capital)
        {
            empireName = name;
            empireReligion = religion;
            knownSystems = knownSys;
            ownedSystems = ownedSys;
            this.fleet = fleet;
            empireRelations = relations;
            capitalPlanet = capital;
            if (rnd == null)
                rnd = new Random();
            missingSecondsToUpdate = rnd.Next(1, 60);
        }

        public void SetEmpireCapital(Planet p)
        {
            if (p.PlanetSettlement.Type == Settlement.SettlementType.CAPITAL)
                capitalPlanet = p;
        }

        public void CreateNewRelation(Empire otherEmpire)
        {
            List<Event> events = new List<Event>();
            //Evento religione
            int i = Religion.ReligionIndex(empireReligion, otherEmpire.EmpireReligion) * 10;
            Event.EventType type;
            if (i < 0)
                type = Event.EventType.NEGATIVE;
            else if (i == 0)
                type = Event.EventType.NEUTRAL;
            else
                type = Event.EventType.POSITIVE;

            events.Add(new Event(type, Event.EventMotivation.RELIGION, i));
            


            //Crea relazione e aggiungila alla lista
            empireRelations.Add(new Relation(otherEmpire.EmpireName, events));
        }

        public Relation ShowRelationWithPlayer()
        {
            string playerName = GameManager.PlayerEmpire.EmpireName;
            foreach(Relation r in EmpireRelations)
            {
                if (r.EmpireName == playerName)
                    return r;
            }
            return new Relation(playerName, new List<Event>());
        }

        public void Update()
        {
            //Se non è ancora tempo di aggiornare ritorna indietro
            if (missingSecondsToUpdate-- > 0)
                return;
            else
                missingSecondsToUpdate = rnd.Next(1, 60);

            if (fleet.Count == 0)
            {
                CreateShip();
            }
            else
                ExploreNewSystem();
            if(empireRelations.Count == 0)
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Metodo che crea una nave per l'impero. Il tipo di nave dipende dalla religione e dalla necessità dell'impero
        /// </summary>
        /// <returns>True se è stata creata, False se no</returns>
        public bool CreateShip()
        {
            //Ottieni il livello della tecnologia
            int lvl = LevelManager.ActualLevel(capitalPlanet.PlanetSettlement.TecnoLevel);
            double lg = Math.Log(lvl + 1); //Le caratteristiche della nave sono create in base 
            int life = (int)(1024 * lg); //al log naturale del livello tecnologico
            int speed = (int)(16 * lg); //per non essere troppo alte una volta arrivati al liv max
            int cargo = (int)(512 * lg);
            int offensive = (int)(32 * lg);

            int c = 5; //Prob di creare una nave commerciale

            switch (empireReligion)
            {
                case Religion.ReligionType.ATEO: //Favorisce navi comm e carico
                    c = 7;
                    cargo = (int)(cargo * 1.5);
                    break;
                case Religion.ReligionType.ATOM: //Favorisce velocità navi (indifferente a tipo di nave)
                    c = 5;
                    speed = (int)(speed * 1.5);
                    break;
                case Religion.ReligionType.BLESS: //Favorisce navi comm e vita
                    c = 7;
                    life = (int)(life * 1.5);
                    break;
                case Religion.ReligionType.CURSER: //Favorisce navi guerra e potenza offensiva
                    c = 3;
                    offensive = (int)(offensive * 1.5);
                    break;
            }
            //Calcola il costo della nave
            int cost = (life + speed + cargo + offensive) / (2 + (int)Math.Abs(Math.Log(1 / LevelManager.ActualLevel(capitalPlanet.PlanetSettlement.ScienceLevel))));
            if (capitalPlanet.PlanetSettlement.Money < cost)
                return false; //Se costa troppo non costruirla

            if (fleet.Count == 0)
                c = 9;

            int i = rnd.Next(0, 10);
            //Se i è minore di c crea la nave commerciale
            if (i < c)
            {
                CommercialShip cs = new CommercialShip(ownedSystems[0].SystemPosition, life, speed, cargo, life);
                fleet.Add(cs);
                capitalPlanet.PlanetSettlement.Money -= cost;
            }
            else
            {
                WarShip ws = new WarShip(ownedSystems[0].SystemPosition, life, speed, offensive, life);
                fleet.Add(ws);
                capitalPlanet.PlanetSettlement.Money -= cost;
            }
            return true;
        }

        /// <summary>
        /// Metodo che manda in esplorazione verso il sistema più vicino una nave dell'impero
        /// </summary>
        /// <returns>True se c'è andata, false se no</returns>
        public bool ExploreNewSystem()
        {
            //Prendi una nave commerciale della flotta e se non è in movimento
            foreach(Ship s in fleet)
            {
                if(s is CommercialShip && s.TargetPosition == s.Position)
                {//Cerca il sistema solare più vicino alla nave
                    float min = float.MaxValue;
                    SolarSystem sis = ownedSystems[0];
                    foreach(SolarSystem ss in GameManager.SolarSystems)
                    {//Che non sia conosciuto o già colonizzato dall'impero
                        if(!knownSystems.Contains(ss) && !ownedSystems.Contains(ss))
                        {
                            float dist = Vector3.DistanceSquared(ss.SystemPosition, s.Position);
                            if(dist < min)
                            {
                                min = dist;
                                sis = ss;
                            }
                        }
                    }
                    //e se lo ha trovato mandala verso quel sistema
                    if(sis != ownedSystems[0])
                    {
                        s.SetTarget(sis.SystemPosition);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Metodo che usa una nave commerciale per terraformare un pianeta del sistema solare in cui si trova
        /// </summary>
        /// <returns>Ritorna true se lo ha terraformato, false se non è stato possibile</returns>
        public bool TerraformPlanet()
        {
            //Prendi una nave commerciale
            foreach(Ship s in fleet)
            {
                if(s is CommercialShip && s.TargetPosition == s.Position)
                {//E controlla se si trova in un sistema solare non abitato o dell'impero
                    foreach(SolarSystem ss in GameManager.SolarSystems)
                    {
                        Empire e = GameManager.SystemOwner(ss);
                        if (ss.SystemPosition == s.Position && (e == null || e.empireName == empireName))
                        {//Controlla i pianeti abitabili se possono essere terraformati
                            if (ss.Planets.Count == 0)
                                break;
                            //Prendi il pianeta con il terrascore più alto
                            Planet pmax = ss.Planets[0];
                            foreach(Planet p in ss.Planets)
                                if(p.IsHabitable && p.Terrascore < 32 && p.Terrascore > pmax.Terrascore)
                                    pmax = p;
                            if(pmax.IsHabitable)
                            {
                                int money = capitalPlanet.PlanetSettlement.Money;
                                int lvl = LevelManager.ActualLevel(pmax.PlanetSettlement.ScienceLevel);

                                int prize = GameActionsManager.TerraformPlanet(pmax, money, lvl, true);
                                capitalPlanet.PlanetSettlement.Money -= prize; //sottrai i soldi dalla capitale
                                if (prize > 0)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
