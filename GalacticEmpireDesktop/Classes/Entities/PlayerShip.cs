using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GalacticEmpire
{
    static class PlayerShip
    {
        static Vector3 position;
        static public Vector3 Position { get { return position; } }

        static Vector3 targetPosition;
        static public Vector3 TargetPosition { get { return targetPosition; } }
        
        static int life;
        static public int Life { get { return life; } }

        static int maxLife;
        static public int MaxLife { get { return maxLife; } }

        static float energy;
        static public float Energy { get { return energy; } }

        static float maxEnergy;
        static public float MaxEnergy { get { return maxEnergy; } }

        static int speed;
        public static int Speed { get { return speed; } }

        static Vector3 actualSpeed;

        static List<Product> products;
        static public List<Product> Products { get { return products; } }

        static int defensiveArmy;
        static public int DefensiveArmy { get { return defensiveArmy; } }

        static int offensiveArmy;
        static public int OffensiveArmy { get { return offensiveArmy; } }

        static int money;
        static public int Money { get { return money; } set { money = value; if (money < 0) money = 0; } }

        static int radius;
        static public int Radius { get { return radius; } }

        static int maxCargo;
        static public int MaxCargo { get { return maxCargo; } }

        static List<PlayerPowerUp> powerUps;
        static public List<PlayerPowerUp> PowerUps { get { return powerUps; } }

        static public bool IsMoving { get { return !(position.X == targetPosition.X && position.Z == targetPosition.Z); } }

        static Model shipModel;
        static float modelRotation;

        static public void Initialize(Vector3 startingPosition, int startingLife, int startingEnergy)
        {
            targetPosition = position = startingPosition;
            maxLife = life = startingLife;
            maxEnergy = energy = (float)startingEnergy;
            modelRotation = 0.0f;
            money = 1000;
            radius = 1500;
            speed = 10;
            maxCargo = 500;
            actualSpeed = new Vector3();
            products = new List<Product>();
            powerUps = new List<PlayerPowerUp>();
        }
        static public void Initialize(Vector3 startingPosition, Vector3 targetPosition, int life, int maxLife, float energy, float maxEnergy,
            int speed, int radius, int money, int maxCargo, List<Product> products, List<PlayerPowerUp> powerUps)
        {
            PlayerShip.targetPosition = targetPosition;
            position = startingPosition;
            PlayerShip.maxLife = maxLife;
            PlayerShip.life = life;
            PlayerShip.maxEnergy = maxEnergy;
            PlayerShip.energy = energy;
            modelRotation = 0.0f;
            PlayerShip.money = money;
            PlayerShip.radius = radius;
            PlayerShip.speed = speed;
            PlayerShip.maxCargo = maxCargo;
            actualSpeed = new Vector3();
            PlayerShip.products = products;
            PlayerShip.powerUps = powerUps;
        }

        static public void Load(Game game)
        {
            shipModel = game.Content.Load<Model>(@"Ship\Starship");
        }

        static public void RestoreLife()
        {
            life = maxLife;
        }

        static public void RestoreEnergy()
        {
            energy = maxEnergy;
        }
        
        static public void SetDestination(Vector3 target)
        {
            Vector3 diff = Vector3.Subtract(target, position);

            if (diff.X * diff.X + diff.Z * diff.Z <= radius * radius)
            {
                targetPosition.X = target.X; targetPosition.Z = target.Z; targetPosition.Y = position.Y;
            }

            float alpha = MathOperations.AngleBetweenVectors(position, target);
           
            actualSpeed.X = speed * (float)Math.Cos(alpha);
            actualSpeed.Z = speed * (float)Math.Sin(alpha);

            if (GameWindow.ActualState == GameWindow.GameState.SYSTEM)
                actualSpeed /= 256;

            if (alpha <= MathHelper.Pi)
                modelRotation = alpha;
            else
                modelRotation = alpha - 2* MathHelper.Pi;
        }

        static public void Update()
        {
            if (Vector3.Distance(position, targetPosition) > 5)
            {
                SetDestination(targetPosition);
                position.X += actualSpeed.X;
                position.Z += actualSpeed.Z;
                energy -= actualSpeed.Length() / 64;
            }
            else
            {
                position = targetPosition;
                actualSpeed = Vector3.Zero;
            }
        }
        
        /// <summary>
        /// Setta la posizione della nave
        /// </summary>
        /// <param name="target">Destinazione</param>
        static public void SetPosition(Vector3 target)
        {
            target.Y = position.Y;
            position = targetPosition = target;
        }

        static public void DrawShip(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            float aspectRatio = GraphicSettings.AspectRatio;

            Matrix[] transforms = new Matrix[shipModel.Bones.Count];
            shipModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh modelmesh in shipModel.Meshes)
            {
                foreach (BasicEffect effect in modelmesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[modelmesh.ParentBone.Index]
                        * Matrix.CreateScale(0.03f)
                        * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(position);

                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);

                }
                modelmesh.Draw();
            }
        }
    }
}
