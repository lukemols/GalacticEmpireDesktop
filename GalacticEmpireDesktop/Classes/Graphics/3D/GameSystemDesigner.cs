using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameSystemDesigner
    {
        static SoundEffect shipStart;
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
            shipStart = game.Content.Load<SoundEffect>(@"Sounds\Effects\ShipStart");
            Model skyboxModel = game.Content.Load<Model>(@"Skybox\Skybox");
            skybox = new Skybox(game.GraphicsDevice, skyboxModel);
            effect = new BasicEffect(game.GraphicsDevice);
            graphicsDevice = game.GraphicsDevice;

            Vector3 cameraPosition = new Vector3(0, 200, 500);
            galaxyCamera = new Camera(cameraPosition, new Vector3(0, 0, 0), 500, MathHelper.PiOver2);
        }

        static public void EnterSystem()
        {
            if (!GameManager.ActualSystem.IsDiscovered && GameManager.ActualSystemOwner != null)
            {
                GameManager.ActualSystemOwner.CreateNewRelation(GameManager.PlayerEmpire);
                GameManager.PlayerEmpire.CreateNewRelation(GameManager.ActualSystemOwner);
            }
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

            Vector2 clickPosition = new Vector2(0, 0);

#if WINDOWS
            clickPosition = Mouse.GetState().Position.ToVector2();
#endif

            if (TouchPanel.GetState().IsConnected)
            {
                TouchCollection touches = TouchPanel.GetState();
                if (touches.Count > 0)
                    foreach (var touch in touches)
                        SetTarget(touch.Position, true);
                else
                    SetTarget(clickPosition, false);
            }
            else
                SetTarget(clickPosition, false);

        }

        static void SetTarget(Vector2 startingPosition, bool FromTouch = false)
        {
            Matrix view = galaxyCamera.CameraView;
            Matrix projection = galaxyCamera.CameraProjection;
            Viewport viewport = graphicsDevice.Viewport;

            foreach (Planet p in GameManager.ActualSystem.Planets)
            {
                if (RayIntersectCalcolator.Intersects(startingPosition, StaticStarModels.GetModel(p.PlanetType), p.GetWorld(), view, projection, viewport))
                {
                    pointedPlanet = p;
                }
            }
            if ((Mouse.GetState().LeftButton == ButtonState.Pressed || FromTouch) && pointedPlanet != null)
            {
                actualPlanet = pointedPlanet;
                PlayerShip.SetDestination(pointedPlanet.PlanetPosition);
                if (GameParams.soundEnabled)
                    shipStart.Play();
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
