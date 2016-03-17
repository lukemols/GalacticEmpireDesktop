using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameGalaxyDesigner
    {
        static SoundEffect shipStart;
        static Skybox skybox;
        static Camera galaxyCamera;
        static BasicEffect effect;
        static GraphicsDevice graphicsDevice;
        static SolarSystem pointedSystem;
        public static SolarSystem PointedSystem { get { return pointedSystem; } }

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

        static public void SetTarget()
        {
            galaxyCamera.UpdateCamera(PlayerShip.Position);
            pointedSystem = null;

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

            foreach (SolarSystem ss in GameManager.SolarSystems)
            {
                if (RayIntersectCalcolator.Intersects(startingPosition, StaticStarModels.GetModel(ss.StarType), ss.GetWorld(), view, projection, viewport))
                {
                    pointedSystem = ss;
                }
            }
            if ((Mouse.GetState().LeftButton == ButtonState.Pressed || FromTouch) && pointedSystem != null)
            {
                PlayerShip.SetDestination(pointedSystem.SystemPosition);
                if (GameParams.soundEnabled)
                    shipStart.Play();
            }
        }

        static public void Draw()
        {
            skybox.Draw(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);

            foreach (SolarSystem ss in GameManager.SolarSystems)
                ss.DrawSystem(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);

            PlayerShip.DrawShip(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);
        }
    }
}
