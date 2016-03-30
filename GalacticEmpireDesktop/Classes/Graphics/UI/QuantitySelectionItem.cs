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
    class QuantitySelectionItem
    {
        Texture2D plusTexture;
        Texture2D minusTexture;
        Texture2D objectTexture;
        Texture2D buttonTexture;
        Texture2D backNumberTexture;

        SpriteFont font;
        int maxValue;
        int pricePerUnit;
        int value;
        int totalPrice;
        public int Value { get { return value; } }
        public int TotalPrice { get { return totalPrice; } }

        List<Button> plusButtons;
        List<Button> minusButtons;
        List<Button> textButtons;
        List<Button> controlButtons;

        Vector2 imagePosition;
        Vector2[] textPosition;

        int lastClickTime;

        public QuantitySelectionItem(Texture2D objectImage, Texture2D pTexture, Texture2D mTexture, Texture2D btTexture,
            Texture2D boxTexture, SpriteFont font)
        {
            plusTexture = pTexture;
            minusTexture = mTexture;
            objectTexture = objectImage;
            buttonTexture = btTexture;
            backNumberTexture = boxTexture;
            this.font = font;
            maxValue = 0;
            pricePerUnit = 0;
            value = 0;
            totalPrice = value * pricePerUnit;

            plusButtons = new List<Button>();
            minusButtons = new List<Button>();
            textButtons = new List<Button>();
            controlButtons = new List<Button>();

            CreateView();
        }

        void CreateView()
        {
            Vector2 startPosition = GraphicSettings.CenterScreen;
            startPosition.X -= (3 * plusTexture.Width + objectTexture.Width + 50);
            startPosition.Y -= (plusTexture.Height + backNumberTexture.Height / 2 + 25);

            imagePosition = new Vector2(startPosition.X, GraphicSettings.CenterScreen.Y - objectTexture.Height / 2);

            int x = (int)startPosition.X + objectTexture.Width + 50;
            int y = (int)startPosition.Y;
            int ym = y + plusTexture.Height + backNumberTexture.Height + 50;
            int yb = y + plusTexture.Height + 25;
            for (int i = 0; i < 6; i++)
            {
                plusButtons.Add(new Button(new Rectangle(x + i * plusTexture.Width, y, plusTexture.Width, plusTexture.Height), "P" + (5 - i)));
                minusButtons.Add(new Button(new Rectangle(x + i * plusTexture.Width, ym, minusTexture.Width, minusTexture.Height), "M" + (5 - i)));
                textButtons.Add(new Button(new Rectangle(x + i * plusTexture.Width, yb, backNumberTexture.Width, backNumberTexture.Height), "B" + (5 - i)));

                plusButtons[i].LoadTexture(plusTexture);
                minusButtons[i].LoadTexture(minusTexture);
                textButtons[i].LoadTextureAndFont(backNumberTexture, font);
                textButtons[i].SetTextColor(Color.Black);
            }
            int xt = x + 6 * plusTexture.Width + 50;
            int yt1 = (int)GraphicSettings.CenterScreen.Y - (int)font.MeasureString("P").Y - 10;
            int yt2 = (int)GraphicSettings.CenterScreen.Y + 10;
            textPosition = new Vector2[] { new Vector2(xt, yt1), new Vector2(xt, yt2) };

            int xbt = (int)GraphicSettings.CenterScreen.X - buttonTexture.Width / 2;
            int ybt = ym + minusTexture.Height + 25;

            controlButtons.Add(new Button(new Rectangle(xbt, ybt, buttonTexture.Width, buttonTexture.Height), "Max", "MaxButton"));
            xbt -= (buttonTexture.Width + 25);
            controlButtons.Add(new Button(new Rectangle(xbt, ybt, buttonTexture.Width, buttonTexture.Height), "Ok", "OkButton"));
            xbt = (int)GraphicSettings.CenterScreen.X + buttonTexture.Width / 2 + 25;
            controlButtons.Add(new Button(new Rectangle(xbt, ybt, buttonTexture.Width, buttonTexture.Height), "Annulla", "CancelButton"));

            foreach (Button b in controlButtons)
            {
                b.LoadTextureAndFont(buttonTexture, font);
                b.SetTextColor(Color.Black);
                b.SetTextPosition();
            }
        }

        public void SetValues(int max, int ppu, Texture2D obj = null)
        {
            if(obj != null)
                objectTexture = obj;
            maxValue = max;
            pricePerUnit = ppu;
            value = 0;
            foreach (Button b in textButtons)
                b.Text = "0";
            lastClickTime = 0;
        }

        public string CheckButtons()
        {
            string button = "NONE";
            if (lastClickTime++ < 10)
                return button;
            foreach(Button b in controlButtons)
            {
                if (b.WasPressed())
                {
                    lastClickTime = 0;
                    button = b.Type;
                    if (button == "MaxButton")
                        value = maxValue;
                }
            }
            foreach (Button bt in plusButtons)
            {
                if (bt.WasPressed())
                {
                    lastClickTime = 0;
                    int i = (int)Char.GetNumericValue(bt.Type, 1);
                    value += (int)Math.Pow(10, i);
                }
            }
            foreach (Button bt in minusButtons)
            {
                if (bt.WasPressed())
                {
                    lastClickTime = 0;
                    int i = (int)Char.GetNumericValue(bt.Type, 1);
                    value -= (int)Math.Pow(10, i);
                }
            }
            if (value < 0)
                value = 0;
            else if (value > maxValue)
                value = maxValue;
            RefreshButtons();
            return button;
        }

        void RefreshButtons()
        {
            totalPrice = value * pricePerUnit;
            for(int i = 0; i < 6; i++)
            {
                textButtons[5 - i].Text = ((value / ((int)Math.Pow(10, i)))%10).ToString();
            }
        }

        public void DrawItem(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(objectTexture, imagePosition, Color.White);
            foreach (Button b in plusButtons)
                b.DrawButton(spriteBatch);
            foreach (Button b in minusButtons)
                b.DrawButton(spriteBatch);
            foreach (Button b in textButtons)
                b.DrawButton(spriteBatch);
            foreach (Button b in controlButtons)
                b.DrawButton(spriteBatch);

            spriteBatch.DrawString(font, "Prezzo per unità: " + pricePerUnit, textPosition[0], Color.Black);
            spriteBatch.DrawString(font, "Costo totale: " + totalPrice, textPosition[1], Color.Black);

        }
    }
}
