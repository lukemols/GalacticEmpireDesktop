using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GalacticEmpire
{
    class Planet
    {
        ///Nome del pianeta
        string name;
        public string Name { get { return name; } }

        ///Tipo di pianeta
        string planetType;
        public string PlanetType { get { return planetType; } }

        Vector3 planetPosition;
        ///Posizione del pianeta
        public Vector3 PlanetPosition { get { return planetPosition; } }

        ///indica se il pianeta è abitabile
        bool isHabitable;
        public bool IsHabitable { get { return isHabitable; } }

        ///Terrascore del pianeta
        int terrascore;
        public int Terrascore { get { return terrascore; } set { if (value <= 32) terrascore = value; } }

        ///Risorse del pianeta
        uint resources;
        public uint Resources { get { return resources; } }

        Settlement planetSettlement;
        public Settlement PlanetSettlement { get { return planetSettlement; } }

        float modelRotation = 0.0f;
        float rotationAroundStar = 0.0f;
        float angleOfRotation;
        float radius;
        Vector3 starPosition;

        public Planet(bool habitable, int score, Vector3 initialPosition, Vector3 starPosition, string type, float radius)
        {
            isHabitable = habitable;
            terrascore = score;
            planetPosition = initialPosition;
            planetType = type;

            name = NameGenerator.GetName(3);

            Random rnd = new Random();
            resources = (uint)(rnd.Next(int.MaxValue / 4, int.MaxValue) * 2); //Questo perché il next vuole degli interi e non degli uint
            //perciò per ottenere il massimo intervallo poi moltiplico per 2

            this.radius = radius;
            this.starPosition = starPosition;
            angleOfRotation = 1 / (8 * radius);
        }

        /// <summary>
        /// Costruttore del pianeta a partire da attributi salvati nel file di salvataggio
        /// </summary>
        /// <param name="Name">Nome</param>
        /// <param name="Position">Posizione</param>
        /// <param name="StarPosition">Posizione della stella intorno alla quale orbita</param>
        /// <param name="PlanetType">Tipo di pianeta</param>
        /// <param name="Habitable">Abitabile</param>
        /// <param name="Terrascore">Terrascore</param>
        /// <param name="Resources">Risorse</param>
        public Planet(string Name, Vector3 Position, Vector3 StarPosition, string PlanetType, bool Habitable, int Terrascore, uint Resources)
        {
            name = Name;
            planetPosition = Position;
            planetType = PlanetType;
            isHabitable = Habitable;
            terrascore = Terrascore;
            resources = Resources;
            starPosition = StarPosition;
            float x = starPosition.X - planetPosition.X;
            float z = starPosition.Z - planetPosition.Z;
            radius = (float)Math.Sqrt(x * x + z * z);
            angleOfRotation = 1 / (8 * radius);
        }

        public void CreateSettlement(Settlement settlement)
        {
            planetSettlement = settlement;
        }

        public void UpdatePosition()
        {
            modelRotation += 0.005f;
            rotationAroundStar += angleOfRotation;
            planetPosition.X = starPosition.X + (radius * (float)Math.Cos(rotationAroundStar));
            planetPosition.Z = starPosition.Z + (radius * (float)Math.Sin(rotationAroundStar));
        }

        public void UpdatePlanet()
        {
            if (planetSettlement != null)
            {
                planetSettlement.Update();
                resources -= planetSettlement.ProduceAndConsume();
            }
        }

        public void DrawPlanet(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            Model starModel = StaticPlanetModels.GetModel(planetType);
            Matrix[] transforms = new Matrix[starModel.Bones.Count];
            starModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh modelmesh in starModel.Meshes)
            {
                foreach (BasicEffect effect in modelmesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[modelmesh.ParentBone.Index]
                        * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(PlanetPosition);

                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicSettings.AspectRatio, 1.0f, 10000.0f);

                }
                modelmesh.Draw();
            }
        }

        public Matrix GetWorld()
        {
            return Matrix.CreateTranslation(planetPosition);
        }
    }
}
