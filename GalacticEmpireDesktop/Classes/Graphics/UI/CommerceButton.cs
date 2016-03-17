using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GalacticEmpire
{
    class CommerceButton<T> : Button
    {
        T buttonComponent;
        public T ButtonComponent { get { return buttonComponent; } }

        string[] strings;
        Texture2D image;
        Texture2D selectedTexture;
        SpriteFont font;
        Vector2 drawingPoint;
        public Vector2 DrawingPoint { set { drawingPoint = value; CreateButton(); } }
        Vector2 texturePoint;
        Vector2[] textPoint;

        int lastClickTime;

        bool isSelected;
        public bool IsSelected { get { return isSelected; } set { if (!value) isSelected = value; } }

        public CommerceButton(Texture2D imageTexture, Texture2D selected, SpriteFont buttonFont, string[] text, Vector2 position, T component) :
            base(new Rectangle((int)position.X, (int)position.Y, 10, 10), "CommerceButton")
        {
            image = imageTexture;
            font = buttonFont;
            strings = text;
            drawingPoint = position;
            selectedTexture = selected;
            buttonComponent = component;
            isSelected = false;
            lastClickTime = 0;
            CreateButton();
        }

        void CreateButton()
        {
            int x = image.Width;
            int y = 0;
            foreach (string s in strings)
            {
                y += (int)font.MeasureString(s).Y;
                int tmp = (int)font.MeasureString(s).X;
                if (x < tmp)
                    x = tmp;
            }
            y += image.Height + 30;
            x += 10;
            Position = new Rectangle((int)drawingPoint.X, (int)drawingPoint.Y, x, y);
            texturePoint = new Vector2((x - image.Width) / 2, 5);
            textPoint = new Vector2[strings.Length];
            int sumY = image.Height + 5;
            for(int i = 0; i< strings.Length; i++)
            {
                int tx = (int)font.MeasureString(strings[i]).X;
                int ty = (int)font.MeasureString(strings[i]).Y;
                textPoint[i] = new Vector2((x - tx) / 2, sumY + 5);
                sumY += ty + 5;
            }
        }

        public new bool WasPressed()
        {
            if (lastClickTime++ < 10)
                return false;
            bool b = WasClicked() || WasTouched();
            if (isSelected && b) //isSelected mantiene il pulsante selezionato finché non ci si clicca sopra per deselezionarlo
                isSelected = false;
            else if (!isSelected && b)
                isSelected = true;
            if (b)
                lastClickTime = 0;
            return b;
        }

        public new void DrawButton(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;
            if (isSelected)
                spriteBatch.Draw(selectedTexture, Position, Color.White);
            spriteBatch.Draw(image, texturePoint + Position.Location.ToVector2(), Color.White);
            for (int i = 0; i < strings.Length; i++)
                spriteBatch.DrawString(font, strings[i], textPoint[i] + Position.Location.ToVector2(), Color.Black);
        }
    }
}
