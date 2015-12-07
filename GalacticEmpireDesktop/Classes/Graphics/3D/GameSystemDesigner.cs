using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameSystemDesigner
    {
        static Skybox skybox;
        static Camera galaxyCamera;
        static BasicEffect effect;
        static GraphicsDevice graphicsDevice;
        static Planet pointedPlanet;
        public static Planet PointedPlanet { get { return pointedPlanet; } }

        static Planet actualPlanet;
        public static Planet ActualPlanet { get { return actualPlanet; } }


        static public void Load(Game game)
        {
            Model skyboxModel = game.Content.Load<Model>(@"Skybox\Skybox");
            skybox = new Skybox(game.GraphicsDevice, skyboxModel);
            effect = new BasicEffect(game.GraphicsDevice);
            graphicsDevice = game.GraphicsDevice;

            GamePlanetUIDesigner.Load(game);


            Vector3 cameraPosition = new Vector3(0, 200, 500);
            galaxyCamera = new Camera(cameraPosition, new Vector3(0, 0, 0), 500, MathHelper.PiOver2);
        }

        static public void EnterSystem()
        {
            if (!GameManager.ActualSystem.IsDiscovered && GameManager.ActualSystemOwner != null)
                GameManager.ActualSystemOwner.CreateNewRelation(GameManager.PlayerEmpire);
            GameManager.ActualSystem.IsDiscovered = true;
            actualPlanet = null;
            foreach(Planet p in GameManager.ActualSystem.Planets)
            {
                if((actualPlanet != null && p.Terrascore > actualPlanet.Terrascore) || (actualPlanet == null && p.Terrascore > 0))
                {
                    actualPlanet = p;
                }
            }
        }

        static public void SetTarget()
        {
            galaxyCamera.UpdateCamera(PlayerShip.Position);
            pointedPlanet = null;
            ///Se il giocatore ha cliccato con il mouse controlla se interseca un pianeta

            Vector2 mouseLocation = Mouse.GetState().Position.ToVector2();
            Matrix view = galaxyCamera.CameraView;
            Matrix projection = galaxyCamera.CameraProjection;
            Viewport viewport = graphicsDevice.Viewport;

            foreach (Planet p in GameManager.ActualSystem.Planets)
            {
                if (RayIntersectCalcolator.Intersects(mouseLocation, StaticStarModels.GetModel(p.PlanetType), p.GetWorld(), view, projection, viewport))
                {
                    pointedPlanet = p;
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pointedPlanet != null)
            {
                actualPlanet = pointedPlanet;
                PlayerShip.SetDestination(pointedPlanet.PlanetPosition);
            }
            else if (actualPlanet != null)
                PlayerShip.SetPosition(actualPlanet.PlanetPosition);
        }

        static public void Draw()
        {
            skybox.Draw(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);

            GameManager.ActualSystem.DrawSystem(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);

            foreach (Planet p in GameManager.ActualSystem.Planets)
                p.DrawPlanet(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);
            
            PlayerShip.DrawShip(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);
            
        }
    }
}
