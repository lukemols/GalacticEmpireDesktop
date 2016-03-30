using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GamePlanetUIDesigner
    {
        public enum PanelState { WAIT, TERRAFORM, COLONIZE, COMMERCE, SPY, EVENTS, COMMERCIALROUTE, ATTACK, GIFT};

        static SpriteFont font;
        static Texture2D bar;
        static Texture2D buttonTexture;
        static Texture2D closeTexture;
        static SpriteBatch spriteBatch;

        static List<Button> inhabitedButtons;
        static Button closeButton;

        static GraphicsDevice device;
        static Texture2D rectangle;
        static Rectangle relativeScreen;
        
        static QuantitySelectionItem selectionBox;

        static PanelState state;
        static int lastClickTime;

        static public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            relativeScreen = new Rectangle(100, 100, GraphicSettings.ScreenBounds.Width - 200, GraphicSettings.ScreenBounds.Height - 200);
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            inhabitedButtons = new List<Button>();

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            bar = game.Content.Load<Texture2D>(GUIFolder + "Bar");
            buttonTexture = game.Content.Load<Texture2D>(GUIFolder + "InGameButton");
            closeTexture = game.Content.Load<Texture2D>(GUIFolder + "Close");

            device = game.GraphicsDevice;

            Color[] data = new Color[(relativeScreen.Width - 100) * (relativeScreen.Height - 100)];
            rectangle = new Texture2D(device, relativeScreen.Width - 100, relativeScreen.Height - 100);

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(200, 200, 200, 10);

            rectangle.SetData(data);
            SetButtons();
            CreateDefaultQuantitySelectionItem(game.Content);

            Initialize();
            //Carica le sottoschede del menu del pianeta
            CommerceUIDesigner.Load(game);
        }

        static void SetButtons()
        {
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;
            closeButton = (new Button(new Rectangle(relativeScreen.Width + relativeScreen.X, relativeScreen.Y, closeTexture.Width, closeTexture.Height), "CloseButton"));
            closeButton.LoadTexture(closeTexture);

            Vector2 buttonDimensions = new Vector2((Width - 200) / 4, (Height - 200) / 5);
            buttonDimensions.X -= 20;
            buttonDimensions.Y -= 10;

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Yellow;
            txt.SetData(data);

            /*
            string[] buttonsText = new string[] { "Commercia", "Rotta commerciale", "Regala", "Eventi", "Spia", "Attacca", "Ripara", "Ricarica" };
            string[] buttonsType = new string[] { "Commerce", "CommercialRoute", "Gift", "Events", "Spy", "Attack", "Repair", "Recharge" };
            for (int i = 0; i < buttonsText.Length; i++)
            {
                int x = relativeScreen.X + relativeScreen.Width / 2 + (i % 2) * relativeScreen.Width / 4 + 10;
                int y = relativeScreen.Y + relativeScreen.Height / 5 + (i / 2) * relativeScreen.Height / 5 + 5;
                Rectangle r = new Rectangle(x, y, txt.Width, txt.Height);
                inhabitedButtons.Add(new Button(r, buttonsText[i], buttonsType[i]));
            }
            */
            string[] buttonsText = new string[] { "Commercia", "Regala", "Ripara", "Ricarica" };
            string[] buttonsType = new string[] { "Commerce", "Gift", "Repair", "Recharge" };
            for (int i = 0; i < buttonsText.Length; i++)
            {
                int x = relativeScreen.X + relativeScreen.Width / 2 + relativeScreen.Width / 4 + 10;
                int y = relativeScreen.Y + relativeScreen.Height / 5 + i * relativeScreen.Height / 5 + 5;
                Rectangle r = new Rectangle(x, y, txt.Width, txt.Height);
                inhabitedButtons.Add(new Button(r, buttonsText[i], buttonsType[i]));
            }

            foreach (Button b in inhabitedButtons)
            {
                b.LoadTextureAndFont(txt, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }
        }

        static void CreateDefaultQuantitySelectionItem(ContentManager Content)
        {
            //public QuantitySelectionItem(Texture2D objectImage, Texture2D pTexture, Texture2D mTexture, Texture2D btTexture,
            //SpriteFont font, int max, int ppu)
            string GUIFolder = GraphicSettings.GetGUIFolder();
            Texture2D pTexture = Content.Load<Texture2D>(GUIFolder + "Plus");
            Texture2D mTexture = Content.Load<Texture2D>(GUIFolder + "Minus");
            Texture2D moneyIcon = Content.Load<Texture2D>(GUIFolder + "Money");

            Vector2 buttonDimensions = new Vector2(pTexture.Width, pTexture.Width);

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D boxTexture = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }
            boxTexture.SetData(data);

            selectionBox = new QuantitySelectionItem(moneyIcon, pTexture, mTexture, buttonTexture, boxTexture, font);
        }


        static public void Initialize()
        {
            lastClickTime = 0;
            state = PanelState.WAIT;
        }

        static public string ButtonClicked()
        {
            string button = "NONE";
            if (lastClickTime++ < 10)
                return button;

            switch(state)
            {
                case PanelState.WAIT:
                    if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                    {
                        foreach (Button b in inhabitedButtons)
                        {
                            if (b.WasPressed())
                            {
                                button = b.Type;
                                LoadNewWindow(button);
                            }
                        }
                    }
                    if (closeButton.WasPressed())
                        button = closeButton.Type;
                    break;
                case PanelState.COMMERCE:
                    if (CommerceUIDesigner.ButtonClicked() == "CloseButton")
                        state = PanelState.WAIT;
                    break;
                case PanelState.GIFT:
                    string s = selectionBox.CheckButtons();
                    if (s == "CancelButton")
                        state = PanelState.WAIT;
                    else if (s == "OkButton")
                    {
                        int gift = selectionBox.TotalPrice;
                        if(GameManager.PlayerEmpire != GameManager.ActualSystemOwner)
                            GameRelationManager.CreateGiftEvent(GameManager.PlayerEmpire, GameManager.ActualSystemOwner, gift);
                        PlayerShip.Money -= gift;
                        GameSystemDesigner.ActualPlanet.PlanetSettlement.Money += gift;
                        state = PanelState.WAIT;
                    }
                    break;
            }

            if (inhabitedButtons[0].WasPressed())
                lastClickTime = 0;
            return button;
        }

        static void LoadNewWindow(string button)
        {
            switch (button)
            {
                case "Commerce":
                    CommerceUIDesigner.LoadWindowComponents(GameSystemDesigner.ActualPlanet);
                    state = PanelState.COMMERCE;
                    break;
                case "Gift":
                    int prize = 1;
                    int max = PlayerShip.Money / prize;
                    selectionBox.SetValues(max, prize);
                    state = PanelState.GIFT;
                    break;
                default:
                    state = PanelState.WAIT;
                    break;
            }
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

            switch(state)
            {
                case PanelState.WAIT:
                    spriteBatch.Draw(rectangle, relativeScreen, Color.White);

                    spriteBatch.DrawString(font, planet.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(planet.Name).X / 2, 110), Color.Red);
                    closeButton.DrawButton(spriteBatch);

                    DrawIfInhabited(planet);
                    break;
                case PanelState.COMMERCE:
                    GameSystemDesigner.Draw();
                    CommerceUIDesigner.Draw();
                    break;
                case PanelState.GIFT:
                    spriteBatch.Draw(rectangle, relativeScreen, Color.White);
                    DrawIfInhabited(planet);
                    selectionBox.DrawItem(spriteBatch);
                    break;
            }

            spriteBatch.End();
        }

        static private void DrawIfInhabited(Planet planet)
        {
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;

            spriteBatch.DrawString(font, "Impero " + GameManager.ActualSystemOwner.EmpireName, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString("Impero " + GameManager.ActualSystemOwner.EmpireName).X / 2, 140), Color.Black);
            //Disegna livello scienza
            spriteBatch.DrawString(font, "Livello scienza: " + LevelManager.ActualLevel(planet.PlanetSettlement.ScienceLevel).ToString(), new Vector2(110, 180), Color.Black);
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
    }
}
