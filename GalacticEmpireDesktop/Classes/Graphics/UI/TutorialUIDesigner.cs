using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    class TutorialUIDesigner
    {
        List<Texture2D> frames;
        SpriteFont font;
        Button forwardBt;
        int lastClickTime;
        int index;

        bool isFinished;
        public bool IsFinished { get { return isFinished; } }

        public void Load(ContentManager content)
        {
            lastClickTime = 0;
            index = 0;
            isFinished = false;
            frames = new List<Texture2D>();

            string GUIFolder = GraphicSettings.GetGUIFolder();
            
            font = content.Load<SpriteFont>(GUIFolder + "Consolas");
            Texture2D buttonTexture = content.Load<Texture2D>(GUIFolder + "InGameButton");
            Rectangle r = new Rectangle((int)(GraphicSettings.CenterScreen.X - buttonTexture.Width / 2),
                GraphicSettings.ScreenBounds.Height - (buttonTexture.Height + 20), buttonTexture.Width, buttonTexture.Height);
            forwardBt = new Button(r, "Avanti", "ForwardButton");
            forwardBt.LoadTextureAndFont(buttonTexture, font);
            forwardBt.SetTextPosition();

            for(int i = 1; i < 7; i++)
            {
                frames.Add(content.Load<Texture2D>("Tutorial/" + i.ToString()));
            }
        }

        public void Update()
        {
            if(lastClickTime++ > 10 && forwardBt.WasPressed())
            {
                lastClickTime = 0;
                index++;
                if (index == frames.Count)
                    isFinished = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(frames[index], GraphicSettings.ScreenBounds, Color.White);
            forwardBt.DrawButton(spriteBatch);
            spriteBatch.End();
        }
    }
}
