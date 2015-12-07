using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GalacticEmpire
{
    class Camera
    {
        float angle = MathHelper.PiOver2;
        int cameraRadius = 500;
        public int CameraRadius { get { return cameraRadius; } }
        int previousWheelValue = 0;
        int previousXMousePosition = 0;

        Vector3 cameraPosition;
        public Vector3 CameraPosition { get { return cameraPosition; } }

        Vector3 cameraTarget;
        public Vector3 CameraTarget { get { return cameraTarget; } }

        Matrix Projection;
        public Matrix CameraProjection { get { return Projection; } }

        Matrix View;
        public Matrix CameraView { get { return View; } }

        public Camera(Vector3 position, Vector3 target, int radius, float startingAngle)
        {
            cameraPosition = position;
            cameraTarget = target;
            cameraRadius = radius;
            angle = startingAngle;
        }

        public void UpdateCamera(Vector3 target)
        {
            cameraTarget = target;
            MouseState state = Mouse.GetState();
            if ((Keyboard.GetState().IsKeyDown(Keys.Z) || state.ScrollWheelValue - previousWheelValue > 0) && cameraRadius > 100)
                cameraRadius -= 25;
            if ((Keyboard.GetState().IsKeyDown(Keys.X) || state.ScrollWheelValue - previousWheelValue < 0) && cameraRadius < 1500)
                cameraRadius += 25;
            if (Keyboard.GetState().IsKeyDown(Keys.Q) || (state.RightButton == ButtonState.Pressed && state.X < previousXMousePosition))
                angle -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) || (state.RightButton == ButtonState.Pressed && state.X > previousXMousePosition))
                angle += 0.01f;

            cameraPosition.X = cameraTarget.X + cameraRadius * (float)Math.Cos(angle);
            cameraPosition.Z = cameraTarget.Z + cameraRadius * (float)Math.Sin(angle);

            previousWheelValue = state.ScrollWheelValue;
            previousXMousePosition = state.X;
            View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            float aspectRatio = GraphicSettings.AspectRatio;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
        }

        public void AutomaticUpdate(float angleMov, int zoom)
        {
            angle += angleMov;
            cameraRadius += zoom;

            cameraPosition.X = cameraTarget.X + cameraRadius * (float)Math.Cos(angle);
            cameraPosition.Z = cameraTarget.Z + cameraRadius * (float)Math.Sin(angle);
            
            View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            float aspectRatio = GraphicSettings.AspectRatio;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
        }

        public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(mousePosition.X, 0, mousePosition.Y);
            Vector3 farPoint = new Vector3(mousePosition.X, 1, mousePosition.Y);

            nearPoint = viewport.Unproject(nearPoint, Projection, View, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, Projection, View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            //direction.Normalize();

            return new Ray(nearPoint, direction);
        }

    }
}
