using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GalacticEmpire
{
    public class Skybox
    {
        GraphicsDevice device;

        Model skyboxModel;
        Matrix[] skyboxTransform;

        public Skybox(GraphicsDevice device, Model skyMod)
        {
            this.device = device;
            skyboxModel = skyMod;

            skyboxTransform = new Matrix[skyboxModel.Bones.Count];
        }

        public void Draw(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            float aspectRatio = GraphicSettings.AspectRatio;

            device.SamplerStates[0] = SamplerState.LinearClamp;
            //Da un valore intero che definisce quale oggetto debba essere disegnato
            device.DepthStencilState = DepthStencilState.None;
            //Si vuole che la skybox sia sempre dietro, quindi devo dirgli che deve essere sempre dietro e quindi di non disegnarli al posto
            //dell'astronave

            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransform);

            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = skyboxTransform[mesh.ParentBone.Index] * Matrix.CreateTranslation(cameraPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 15000.0f);
                }
                mesh.Draw();
            }
        }
    }
}
