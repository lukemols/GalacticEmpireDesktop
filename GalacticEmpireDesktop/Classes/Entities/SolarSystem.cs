using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GalacticEmpire
{
    class SolarSystem
    {
        static Random rnd;

        ///Nome del sistema solare
        string name;
        public string Name { get { return name; } }

        ///Tipo di stella
        private string starType;
        public string StarType { get { return starType; } }
        
        ///Posizione del sistema solare
        Vector3 systemPosition;
        public Vector3 SystemPosition { get { return systemPosition; } }

        ///Lista di pianeti
        List<Planet> planets;
        public List<Planet> Planets { get { return planets; } }

        bool isDiscovered;
        public bool IsDiscovered { get { return isDiscovered; } set { if (value) isDiscovered = value; } }

        bool isInhabited;
        public bool IsInhabited { get { return isDiscovered; } set { if (value) isInhabited = value; } }

        float modelRotation = 0.0f;

        /// <summary>
        /// Costruttore del Sistema Solare per un nuovo gioco
        /// </summary>
        /// <param name="position">Posizione del Sistema Solare</param>
        public SolarSystem(Vector3 position)
        {
            isDiscovered = false;
            isInhabited = false;
            systemPosition = position;
            name = NameGenerator.GetName(2);
            //Scegli il tipo di stella
            if (rnd == null)
                rnd = new Random();
            int x = rnd.Next(0, 128);
            x = this.GetHashCode() % 128;
            
            if ((x -= 64) < 0)
                starType = "Red";
            else if ((x -= 32) < 0)
                starType = "Orange";
            else if ((x -= 16) < 0)
                starType = "Yellow";
            else if ((x -= 8) < 0)
                starType = "YellowWhite";
            else if ((x -= 4) < 0)
                starType = "White";
            else if ((x -= 2) < 0)
                starType = "BlueWhite";
            else
                starType = "Blue";
        }

        public void CreatePlanets(bool playerStartingSystem = false)
        {
            planets = new List<Planet>();
            for (int i = 1; i < 10; i++)
            {
                if (rnd.Next(0, 3) != 0) // non si crea il pianeta
                    continue;
                bool habitable = i < 4 || i > 6 ? false : true; //Se i è tra 4 e 6 il pianeta è abitabile
                int score = habitable ? rnd.Next(0, 32) : 0; //e gli si da un terrascore
                Vector3 startPosition = systemPosition;
                startPosition.X += i * 60; //posizione iniziale del pianeta
                string type;
                if (i <= 6)
                    type = "Type" + rnd.Next(1, 10); //Tipo del pianeta, se in posizione fino a 6 è roccioso, altrimenti gassoso
                else
                    type = "Gas" + rnd.Next(1, 9);
                Planet p = new Planet(habitable, score, startPosition, systemPosition, type, i * 60);//Crea il pianeta
                planets.Add(p); //ed aggiungilo alla lista
            }
            if(playerStartingSystem)
            {
                bool created = false;
                foreach(Planet p in planets)
                    if(p.IsHabitable)
                    {
                        p.Terrascore = 32;
                        Settlement settlement = new Settlement(Settlement.SettlementType.CAPITAL, GameParams.religionType);
                        p.CreateSettlement(settlement);
                        created = true;
                        break;
                    }
                if(!created)
                {
                    Vector3 startPosition = systemPosition;
                    startPosition.X += rnd.Next(4,7) * 60; //posizione iniziale del pianeta
                    string type = "Type" + rnd.Next(1, 10); //Tipo del pianeta, se in posizione fino a 6 è roccioso, altrimenti gassoso
                    
                    Planet p = new Planet(true, 32, startPosition, systemPosition, type, startPosition.X - systemPosition.X);//Crea il pianeta
                    Settlement settlement = new Settlement(Settlement.SettlementType.CAPITAL, GameParams.religionType);
                    p.CreateSettlement(settlement);
                    planets.Add(p); //ed aggiungilo alla lista
                }
            }
        }

        public void UpdatePosition()
        {
            modelRotation += 0.005f;
            if (modelRotation > MathHelper.TwoPi) //Fai girare la stella e se supera l'angolo di 2pi falla tornare a zero
                modelRotation = 0.0f;

            foreach (Planet p in planets)
            {
                p.UpdatePosition();
            }
        }

        public void UpdateSystem()
        {
            foreach (Planet p in planets)
                p.UpdatePlanet();
            
        }

        public void DrawSystem(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            Model starModel = StaticStarModels.GetModel(starType);
            Matrix[] transforms = new Matrix[starModel.Bones.Count];
            starModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh modelmesh in starModel.Meshes)
            {
                foreach (BasicEffect effect in modelmesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[modelmesh.ParentBone.Index]
                        * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(systemPosition);

                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicSettings.AspectRatio, 1.0f, 10000.0f);

                }
                modelmesh.Draw();
            }
        }

        public Matrix GetWorld()
        {
            return Matrix.CreateTranslation(systemPosition);
        }
    }
}
