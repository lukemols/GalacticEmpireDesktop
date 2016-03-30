using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GamePlanetDisabitedUIDesigner
    {
        public enum PanelState { WAIT, TERRAFORM, FOUNDCOLONY, AWAITCONFIRM}

        static SpriteFont font;
        static Texture2D bar;
        static Texture2D buttonTexture;
        static Texture2D closeTexture;
        static SpriteBatch spriteBatch;

        static List<Button> dishabitedButtons;
        static List<Button> colonyButtons;
        static List<Button> ChoiceButtons;
        static Button closeButton;

        static GraphicsDevice device;
        static Texture2D rectangle;
        static Rectangle relativeScreen;

        static QuantitySelectionItem selectionBox;

        static PanelState state;
        static int lastClickTime;

        static int prize;
        static string settlementType;

        static public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            relativeScreen = new Rectangle(100, 100, GraphicSettings.ScreenBounds.Width - 200, GraphicSettings.ScreenBounds.Height - 200);
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            dishabitedButtons = new List<Button>();
            colonyButtons = new List<Button>();
            ChoiceButtons = new List<Button>();

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            bar = game.Content.Load<Texture2D>(GUIFolder + "Bar");
            closeTexture = game.Content.Load<Texture2D>(GUIFolder + "Close");
            buttonTexture = game.Content.Load<Texture2D>(GUIFolder + "InGameButton");

            device = game.GraphicsDevice;

            Color[] data = new Color[(relativeScreen.Width - 100) * (relativeScreen.Height - 100)];
            rectangle = new Texture2D(device, relativeScreen.Width - 100, relativeScreen.Height - 100);

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(200, 200, 200, 10);

            rectangle.SetData(data);
            SetButtons();
            CreateDefaultQuantitySelectionItem(game.Content);
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

            string[] buttonsText = new string[] { "Crea colonia", "Terraforma"};
            string[] buttonsType = new string[] { "Found", "Terraform"};
            for (int i = 0; i < buttonsText.Length; i++)
            {
                int x = relativeScreen.X + 3 * relativeScreen.Width / 4 + 10;
                int y = relativeScreen.Y + relativeScreen.Height / 5 + i * relativeScreen.Height / 5 + 5;
                Rectangle r = new Rectangle(x, y, txt.Width, txt.Height);
                dishabitedButtons.Add(new Button(r, buttonsText[i], buttonsType[i]));

            }
            foreach (Button b in dishabitedButtons)
            {
                b.LoadTextureAndFont(txt, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }

            buttonDimensions = new Vector2(Width / 10, Height / 16);

            data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(64, 64, 128, 128);
            txt.SetData(data);

            ChoiceButtons.Add(new Button(new Rectangle(), "Crea colonia", "YesButton"));
            ChoiceButtons[0].LoadTexture(txt);
            ChoiceButtons.Add(new Button(new Rectangle(), "Annulla", "CancelButton"));
            ChoiceButtons[1].LoadTexture(txt);

            int w = ChoiceButtons[0].ButtonTexture.Width;
            int h = ChoiceButtons[0].ButtonTexture.Height;
            int s = 25;
            int X = (int)GraphicSettings.CenterScreen.X - (w + s);
            int j = 0;
            foreach (Button b in ChoiceButtons)
            {
                b.Position = new Rectangle(X + j * w + j * s, (int)GraphicSettings.CenterScreen.Y, w, h);
                b.SetTextColor(Color.Black);
                j++;
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

        static void CreateColonyButtons()
        {
            string[] buttonsText;
            string[] buttonsType;
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;

            Vector2 buttonDimensions = new Vector2((Width - 200) / 4, (Height - 200) / 5);
            buttonDimensions.X -= 20;
            buttonDimensions.Y -= 10;

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Yellow;
            txt.SetData(data);

            if (GameSystemDesigner.ActualPlanet.Terrascore > 24)
            {
                buttonsText = new string[] { "Città", "Colonia", "Insediamento militare" };
                buttonsType = new string[] { "Community", "Inhabited", "Military" };
            }
            else if (GameSystemDesigner.ActualPlanet.Terrascore < 16)
            {
                buttonsText = new string[] { "Insediamento militare" };
                buttonsType = new string[] { "Military" };
            }
            else
            {
                buttonsText = new string[] { "Colonia", "Insediamento militare" };
                buttonsType = new string[] { "Inhabited", "Military" };
            }

            for (int i = 0; i < buttonsText.Length; i++)
            {
                int x = relativeScreen.X + 3 * relativeScreen.Width / 4 + 10;
                int y = relativeScreen.Y + relativeScreen.Height / 5 + i * relativeScreen.Height / 5 + 5;
                Rectangle r = new Rectangle(x, y, txt.Width, txt.Height);
                colonyButtons.Add(new Button(r, buttonsText[i], buttonsType[i]));
            }

            foreach (Button b in colonyButtons)
            {
                b.LoadTextureAndFont(txt, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }
        }


        static public void Initialize()
        {
            lastClickTime = 0;
            prize = int.MaxValue;
            settlementType = "";
            state = PanelState.WAIT;
            CreateColonyButtons();
        }

        static public string ButtonClicked()
        {
            string button = "NONE";
            if (lastClickTime++ < 10)
                return button;

            switch(state)
            {
                case PanelState.FOUNDCOLONY:
                    foreach(Button b in colonyButtons)
                    {
                        if (b.WasPressed())
                        {
                            button = b.Type;
                            prize = int.MaxValue;
                            settlementType = button;
                            ShowQuestionCreateSettlement();
                            break;
                        } 
                    }
                    if (closeButton.WasPressed())
                        state = PanelState.WAIT;
                    break;
                case PanelState.AWAITCONFIRM:
                    if (CreateSettlement())
                        button = closeButton.Type;
                    break;
                case PanelState.TERRAFORM:
                    string s = selectionBox.CheckButtons();
                    if (s == "CancelButton")
                        state = PanelState.WAIT;
                    else if (s == "OkButton")
                    {
                        int prize = selectionBox.TotalPrice;
                        int value = selectionBox.Value;
                        PlayerShip.Money -= prize;
                        GameSystemDesigner.ActualPlanet.Terrascore += value;
                        state = PanelState.WAIT;
                    }
                    button = s;
                    break;
                case PanelState.WAIT:
                default:
                    foreach (Button b in dishabitedButtons)
                        if (b.WasPressed())
                        {
                            button = b.Type;
                            LoadNewWindow(button);
                        }
                    if (closeButton.WasPressed())
                        button = closeButton.Type;
                    break;
            }
            if (button != "NONE")
                lastClickTime = 0;
            return button;
        }

        static void LoadNewWindow(string button)
        {
            switch (button)
            {
                case "Terraform":
                    int level = PlayerShip.GetMaxPowerUpLevel(PlayerPowerUp.PowerUpType.TERRAFORMER);
                    if (level <= 0)
                        level = 1;
                    int prize = GameActionsManager.TerraformPlanet(GameSystemDesigner.ActualPlanet, PlayerShip.Money, level);
                    if (PlayerShip.Money < prize || prize == 0)
                        break;
                    int max = PlayerShip.Money / prize;
                    max = MathHelper.Min(max, 32 - GameSystemDesigner.ActualPlanet.Terrascore);
                    selectionBox.SetValues(max, prize);
                    state = PanelState.TERRAFORM;
                    break;
                case "Found":
                    state = PanelState.FOUNDCOLONY;
                    break;
                default:
                    break;
            }
        }

        static void ShowQuestionCreateSettlement()
        {
            Settlement.SettlementType type = Settlement.GetSettlementTypeFromString(settlementType);
            prize = GameActionsManager.GetPrizeToCreateColony(type);
            if (prize <= PlayerShip.Money)
                state = PanelState.AWAITCONFIRM;
        }

        static bool CreateSettlement()
        {
            bool result = false;
            foreach(Button b in ChoiceButtons)
            {
                if(b.WasPressed())
                {
                    if(b.Type == "YesButton")
                    {
                        Settlement.SettlementType type = Settlement.GetSettlementTypeFromString(settlementType);
                        Settlement capitalSett = GameManager.PlayerCapital.PlanetSettlement;
                        Settlement s = new Settlement(type, prize / 2, capitalSett.Inhabitants / 16, capitalSett.DefensiveArmy, capitalSett.OffensiveArmy,
                            capitalSett.InhaGrowthRate, capitalSett.ScienceLevel, capitalSett.ScienceGrowthRate, capitalSett.CommerceLevel, capitalSett.CommerceGrowthRate,
                            capitalSett.TecnoLevel, capitalSett.TecnoGrowthRate);
                        GameSystemDesigner.ActualPlanet.CreateSettlement(s);
                        GameManager.SetActualSystemOwner(GameManager.PlayerEmpire);
                        PlayerShip.Money -= prize;
                        state = PanelState.WAIT;
                        settlementType = "";
                        prize = int.MaxValue;
                        return true;
                    }
                    else if(b.Type == "CancelButton")
                    {
                        state = PanelState.WAIT;
                        settlementType = "";
                        prize = int.MaxValue;
                        return false;
                    }
                }
            }
            return result;
        }
        
        static public void Draw(Planet planet)
        {
            spriteBatch.Begin();
            
            spriteBatch.Draw(rectangle, relativeScreen, Color.White);
            spriteBatch.DrawString(font, planet.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(planet.Name).X / 2, 110), Color.Red);
            closeButton.DrawButton(spriteBatch);

            spriteBatch.DrawString(font, GameManager.ActualSystem.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(GameManager.ActualSystem.Name).X / 2, 140), Color.Black);

            switch(state)
            {
                case PanelState.WAIT:
                    DrawBackground(planet);
                    DrawButtons(dishabitedButtons);
                    break;
                case PanelState.TERRAFORM:
                    DrawBackground(planet);
                    DrawButtons(dishabitedButtons);
                    selectionBox.DrawItem(spriteBatch);
                    break;
                case PanelState.FOUNDCOLONY:
                    DrawBackground(planet);
                    DrawButtons(colonyButtons);
                    break;
                case PanelState.AWAITCONFIRM:
                    DrawBackground(planet);
                    DrawButtons(ChoiceButtons);
                    string txt = "Per creare questa colonia dovrai pagare " + prize + ". Confermi?";
                    int xt = (int)font.MeasureString(txt).X;
                    int yt = (int)font.MeasureString(txt).Y;
                    int cx = (int)GraphicSettings.CenterScreen.X;
                    int cy = (int)GraphicSettings.CenterScreen.Y;
                    spriteBatch.DrawString(font, txt, new Vector2(cx - xt / 2, cy - yt - 10), Color.Black);
                    break;
            }
            

            spriteBatch.End();
        }

        static void DrawBackground(Planet planet)
        {
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
            }
            else
            {
                string txt = "Questo pianeta non è colonizzabile poiché\nun altro impero è il proprietario del sistema.";
                spriteBatch.DrawString(font, txt, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(txt).X / 2, 180), Color.Black);
            }
        }

        static void DrawButtons(List<Button> buttons)
        {
            foreach (Button b in buttons)
                b.DrawButton(spriteBatch);
        }

    }
}
