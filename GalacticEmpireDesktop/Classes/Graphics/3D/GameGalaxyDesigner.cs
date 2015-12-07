using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameGalaxyDesigner
    {
        static Skybox skybox;
        static Camera galaxyCamera;
        static BasicEffect effect;
        static GraphicsDevice graphicsDevice;
        static SolarSystem pointedSystem;
        public static SolarSystem PointedSystem { get { return pointedSystem; } }

        static public void Load(Game game)
        {
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

            ///Se il giocatore ha cliccato con il mouse controlla se interseca una stella

            Vector2 mouseLocation = Mouse.GetState().Position.ToVector2();
            Matrix view = galaxyCamera.CameraView;
            Matrix projection = galaxyCamera.CameraProjection;
            Viewport viewport = graphicsDevice.Viewport;

            foreach (SolarSystem ss in GameManager.SolarSystems)
            {
                if (RayIntersectCalcolator.Intersects(mouseLocation, StaticStarModels.GetModel(ss.StarType), ss.GetWorld(), view, projection, viewport))
                {
                    pointedSystem = ss;
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pointedSystem != null)
                PlayerShip.SetDestination(pointedSystem.SystemPosition);
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
