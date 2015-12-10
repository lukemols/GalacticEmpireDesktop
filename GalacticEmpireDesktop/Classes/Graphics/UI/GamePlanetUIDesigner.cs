using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GamePlanetUIDesigner
    {
        static SpriteFont font;
        static Texture2D bar;
        static Texture2D buttonTexture;
        static Texture2D closeTexture;
        static Texture2D foodIcon;
        static SpriteBatch spriteBatch;

        static List<Button> inhabitedButtons;
        static List<Button> dishabitedButtons;
        static Button closeButton;

        static GraphicsDevice device;
        static Texture2D rectangle;
        static Rectangle relativeScreen;

        static public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            relativeScreen = new Rectangle(100, 100, GraphicSettings.ScreenBounds.Width - 100, GraphicSettings.ScreenBounds.Height - 100);
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            inhabitedButtons = new List<Button>();
            dishabitedButtons = new List<Button>();

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            bar = game.Content.Load<Texture2D>(GUIFolder + "Bar");
            buttonTexture = game.Content.Load<Texture2D>(GUIFolder + "InGameButton");
            closeTexture = game.Content.Load<Texture2D>(GUIFolder + "Close");
            foodIcon = game.Content.Load<Texture2D>(GUIFolder + "FoodIcon");

            device = game.GraphicsDevice;

            Color[] data = new Color[(relativeScreen.Width - 100) * (relativeScreen.Height - 100)];
            rectangle = new Texture2D(device, relativeScreen.Width - 100, relativeScreen.Height - 100);

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(200, 200, 200, 10);

            rectangle.SetData(data);
            SetButtons();
        }

        static void SetButtons()
        {
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;
            closeButton = (new Button(new Rectangle(relativeScreen.Width, relativeScreen.Y, closeTexture.Width, closeTexture.Height), "CloseButton"));
            closeButton.LoadTexture(closeTexture);

            Vector2 buttonDimensions = new Vector2((Width - 200) / 3, (Height - 250) / 5);

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Yellow;
            txt.SetData(data);

            inhabitedButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 50, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Commercia", "Commerce"));
            inhabitedButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 60 + (int)buttonDimensions.Y, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Ripara", "Repair"));
            inhabitedButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 70 + 2 * (int)buttonDimensions.Y, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Ricarica", "Recharge"));

            dishabitedButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 50, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Terraforma", "Terraform"));
            dishabitedButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 60 + (int)buttonDimensions.Y, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Fonda colonia", "Found"));

            foreach (Button b in inhabitedButtons)
            {
                b.LoadTextureAndFont(txt, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }
            foreach (Button b in dishabitedButtons)
            {
                b.LoadTextureAndFont(txt, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }
        }
        static public string ButtonClicked()
        {
            string button = "NONE";
            if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
            {
                foreach (Button b in inhabitedButtons)
                {
                    if (b.WasPressed())
                        button = b.Type;
                }
            }
            else
            {
                foreach (Button b in dishabitedButtons)
                {
                    if (b.WasPressed())
                        button = b.Type;
                }
            }
            if (closeButton.WasPressed())
                button = closeButton.Type;
            return button;
        }

        static public void WriteInhabitedButtons()
        {
            foreach(Button b in inhabitedButtons)
            {
                switch(b.Type)
                {
                    case "Recharge":
                        b.Text = "Ricarica:\n";
                        int prize = GameCommerceManager.GetRechargePrize();
                        if (prize == 0)
                            b.Text += "Gratis";
                        else
                            b.Text += prize.ToString() + " Crediti";
                        break;
                    case "Repair":
                        b.Text = "Ripara:\n";
                        int repPrize = GameCommerceManager.GetRepairPrize();
                        if (repPrize == 0)
                            b.Text += "Gratis";
                        else
                            b.Text += repPrize.ToString() + " Crediti";
                        break;
                }
            }
        }

        static public void Draw(Planet planet)
        {
            spriteBatch.Begin();

            Vector2 position = new Vector2(100, 100);
            spriteBatch.Draw(rectangle, position, Color.White);

            spriteBatch.DrawString(font, planet.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(planet.Name).X / 2, 110), Color.Red);
            closeButton.DrawButton(spriteBatch);

            if (GameWindow.ActualState == GameWindow.GameState.PLANET)
            {
                if (planet.PlanetSettlement != null)
                    DrawIfInhabited(planet);
                else if (planet.IsHabitable)
                    DrawIfHabitable(planet);
            }
            else if (GameWindow.ActualState == GameWindow.GameState.COMMERCE)
                DrawCommerce(planet);

            spriteBatch.End();
        }

        static private void DrawIfInhabited(Planet planet)
        {
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;

            spriteBatch.DrawString(font, "Impero " + GameManager.ActualSystemOwner.EmpireName, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString("Impero " + GameManager.ActualSystemOwner.EmpireName).X / 2, 140), Color.Black);
            //Disegna livello scienza
            spriteBatch.DrawString(font, "Livello scienza: " + LevelManager.ActualLevel(planet.PlanetSettlement.ScienceLevel).ToString() + " - " + planet.PlanetSettlement.ScienceLevel.ToString(), new Vector2(110, 180), Color.Black);
            spriteBatch.Draw(bar, new Vector2(110, 200), Color.Gray);
            spriteBatch.Draw(bar, new Rectangle(110, 200, (int)((float)bar.Width * LevelManager.PercentLevel(planet.PlanetSettlement.ScienceLevel)), bar.Height), Color.LimeGreen);
            //Disegna livello commercio
            spriteBatch.DrawString(font, "Livello commercio: " + LevelManager.ActualLevel(planet.PlanetSettlement.CommerceLevel).ToString(), new Vector2(110, 230), Color.Black);
            spriteBatch.Draw(bar, new Vector2(110, 250), Color.Gray);
            spriteBatch.Draw(bar, new Rectangle(110, 250, (int)((float)bar.Width * LevelManager.PercentLevel(planet.PlanetSettlement.CommerceLevel)), bar.Height), Color.MonoGameOrange);
            //Disegna livell tecnologico
            spriteBatch.DrawString(font, "Livello tecnologico: " + LevelManager.ActualLevel(planet.PlanetSettlement.TecnoLevel).ToString(), new Vector2(110, 280), Color.Black);
            spriteBatch.Draw(bar, new Vector2(110, 300), Color.Gray);
            spriteBatch.Draw(bar, new Rectangle(110, 300, (int)((float)bar.Width * LevelManager.PercentLevel(planet.PlanetSettlement.TecnoLevel)), bar.Height), Color.MidnightBlue);
            //Disegna il numero di abitanti
            spriteBatch.DrawString(font, "Abitanti: " + ((long)planet.PlanetSettlement.Inhabitants * 1000).ToString("#,0."), new Vector2(110, 330), Color.Black);

            spriteBatch.DrawString(font, "Risorse: " + planet.Resources.ToString("#,0."), new Vector2(110, 360), Color.Black);

            //Disegna livello terrascore
            spriteBatch.DrawString(font, "Terrascore: " + planet.Terrascore.ToString(), new Vector2(110, 390), Color.Black);
            spriteBatch.Draw(bar, new Vector2(110, 410), Color.Gray);
            Color barColor;
            if(planet.Terrascore < 16)
                barColor = Color.Red;
            else if (planet.Terrascore < 24)
                barColor = Color.Yellow;
            else
                barColor = Color.LimeGreen;
            spriteBatch.Draw(bar, new Rectangle(110, 410, bar.Width * planet.Terrascore / 32, bar.Height), barColor);

            foreach (Button b in inhabitedButtons)
                b.DrawButton(spriteBatch);

            
        }


        static private void DrawIfHabitable(Planet planet)
        {
            spriteBatch.DrawString(font, GameManager.ActualSystem.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(GameManager.ActualSystem.Name).X / 2, 140), Color.Black);

            if (GameManager.ActualSystemOwner == null || GameManager.ActualSystemOwner == GameManager.PlayerEmpire)
            {
                //Risorse
                spriteBatch.DrawString(font, "Risorse: " + planet.Resources.ToString("#,0."), new Vector2(110, 180), Color.Black);
                //Disegna livello terrascore
                spriteBatch.DrawString(font, "Terrascore: " + planet.Terrascore.ToString(), new Vector2(110, 230), Color.Black);
                spriteBatch.Draw(bar, new Vector2(110, 250), Color.Gray);
                Color barColor;
                if (planet.Terrascore < 16)
                    barColor = Color.Red;
                else if (planet.Terrascore < 24)
                    barColor = Color.Yellow;
                else
                    barColor = Color.LimeGreen;
                spriteBatch.Draw(bar, new Rectangle(110, 250, bar.Width * planet.Terrascore / 32, bar.Height), barColor);
                foreach (Button b in dishabitedButtons)
                    b.DrawButton(spriteBatch);
            }
            else
            {
                string txt = "Questo pianeta non e' colonizzabile poiche'\nun altro impero e' il proprietario del sistema.";
                spriteBatch.DrawString(font, txt, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(txt).X / 2, 180), Color.Black);
            }
        }

        static private void DrawCommerce(Planet planet)
        {
            int x = GraphicSettings.ScreenBounds.Width - 200;
            x /= foodIcon.Width;

        }
    }
}
