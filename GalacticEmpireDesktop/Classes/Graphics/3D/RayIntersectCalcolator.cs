﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class RayIntersectCalcolator
    {
        static private Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 1.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        static private float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(sphere);
        }

        static public bool Intersects(Vector2 mouseLocation, Model model, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            for (int index = 0; index < model.Meshes.Count; index++)
            {
                BoundingSphere sphere = model.Meshes[index].BoundingSphere;
                sphere = sphere.Transform(world);
                float? distance = IntersectDistance(sphere, mouseLocation, view, projection, viewport);

                if (distance != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
