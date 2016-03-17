using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class GameUIDesigner
    {
        static SpriteFont font;
        static Texture2D bar;
        static Texture2D heart;
        static Texture2D power;
        static Texture2D money;
        static List<Button> OnScreenButtons;
        static Button pointedButton;
        static SpriteBatch spriteBatch;

        static public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            OnScreenButtons = new List<Button>();
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            bar = game.Content.Load<Texture2D>(GUIFolder + "Bar");

            heart = game.Content.Load<Texture2D>(GUIFolder + "Heart");
            power = game.Content.Load<Texture2D>(GUIFolder + "Power");
            money = game.Content.Load<Texture2D>(GUIFolder + "Money");

            pointedButton = new Button(new Rectangle(), "", "PointedButton");
            pointedButton.LoadTextureAndFont(game.Content.Load<Texture2D>(GUIFolder + "InGameButton"), font);

            OnScreenButtons.Add(new Button(new Rectangle(), "CameraButton"));
            OnScreenButtons[0].LoadTexture(game.Content.Load<Texture2D>(GUIFolder + "CameraButton"));
            OnScreenButtons.Add(new Button(new Rectangle(), "PauseButton"));
            OnScreenButtons[1].LoadTexture(game.Content.Load<Texture2D>(GUIFolder + "PauseButton"));
            OnScreenButtons.Add(new Button(new Rectangle(), "Torna alla\n galassia", "ReturnToGalaxy"));
            OnScreenButtons[2].LoadTexture(game.Content.Load<Texture2D>(GUIFolder + "InGameButton"));
            SetButtonPositions();
        }

        static private void SetButtonPositions()
        {
            int height = GraphicSettings.ScreenBounds.Height;
            int width = GraphicSettings.ScreenBounds.Width;

            pointedButton.Position = new Rectangle(10, height - 100, pointedButton.ButtonTexture.Width, pointedButton.ButtonTexture.Height);
            int w = OnScreenButtons[0].ButtonTexture.Width;
            foreach (Button b in OnScreenButtons)
            {
                switch(b.Type)
                {
                    case "CameraButton":
                        b.Position = new Rectangle(width - 20 - b.ButtonTexture.Width * 2, 10, b.ButtonTexture.Width, b.ButtonTexture.Height);
                        break;
                    case "PauseButton":
                        b.Position = new Rectangle(width - 20 - b.ButtonTexture.Width, 10, b.ButtonTexture.Width, b.ButtonTexture.Height);
                        break;
                    case "ReturnToGalaxy":
                        b.Position = new Rectangle(width - 20 - b.ButtonTexture.Width - 2 * w, 10, b.ButtonTexture.Width, b.ButtonTexture.Height);
                        break;
                }
            }
        }
        
        static public string ButtonClicked()
        {
            string button = "NONE";
            foreach(Button b in OnScreenButtons)
            {
                if (b.WasPressed())
                    button = b.Type;
            }
            if (pointedButton.WasPressed())
                button = pointedButton.Type;
            return button;
        }

        static public void Draw()
        {
            spriteBatch.Begin();
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;

            int w = bar.Width + power.Width + 20;
            int h = power.Height + 10;
            //Barra della vita
            spriteBatch.Draw(bar, new Rectangle(Width - bar.Width - 10, Height - (h + 10 + bar.Height / 2 + power.Height / 2), bar.Width, bar.Height), Color.Gray);
            spriteBatch.Draw(bar, new Rectangle(Width - bar.Width - 10, Height - (h + 10 + bar.Height / 2 + power.Height / 2), bar.Width * PlayerShip.Life / PlayerShip.MaxLife, bar.Height), Color.Red);
            //Barra dell'energia
            spriteBatch.Draw(bar, new Rectangle(Width - bar.Width - 10, Height - (bar.Height / 2 + power.Height / 2 + 10), bar.Width, bar.Height), Color.Gray);
            spriteBatch.Draw(bar, new Rectangle(Width - bar.Width - 10, Height - (bar.Height / 2 + power.Height / 2 + 10), (int)((float)bar.Width * (PlayerShip.Energy / PlayerShip.MaxEnergy)), bar.Height), Color.Blue);
            //Simboli del cuore e della potenza
            spriteBatch.Draw(heart, new Vector2(Width - w, Height - 2 * h), Color.White);
            spriteBatch.Draw(power, new Vector2(Width - w, Height - h), Color.White);
            //Soldi del giocatore
            spriteBatch.Draw(money, new Vector2(Width - w, Height - (3 * h)), Color.White);
            spriteBatch.DrawString(font, PlayerShip.Money.ToString("#,0."), new Vector2(Width - font.MeasureString(PlayerShip.Money.ToString("#,0.")).X - 10, Height - (10 + 2 * h + money.Height / 2 + font.MeasureString(PlayerShip.Money.ToString("#,0.")).Y / 2)), Color.Wheat);

            //Disegna i pulsanti sullo schermo
            foreach (Button b in OnScreenButtons)
                if(!(b.Type == "ReturnToGalaxy" && GameWindow.ActualState != GameWindow.GameState.SYSTEM))
                   b.DrawButton(spriteBatch);
            
            // GUI relativa alla parte di gioco nel sistema solare
            if (GameWindow.ActualState == GameWindow.GameState.SYSTEM)
            {
                // Scrivi il nome del pianeta puntato sopra di esso
                if (GameSystemDesigner.PointedPlanet != null)
                    spriteBatch.DrawString(font, GameSystemDesigner.PointedPlanet.Name, new Vector2(Mouse.GetState().Position.ToVector2().X, Mouse.GetState().Position.ToVector2().Y - 20), Color.White);
                if (GameSystemDesigner.ActualPlanet != null)
                {
                    //Scrivi il nome del pianeta attuale nel pulsante
                    pointedButton.Text = GameSystemDesigner.ActualPlanet.Name;
                    int hei = GraphicSettings.ScreenBounds.Height - pointedButton.ButtonTexture.Height - 50;
                    // e notifica il fatto se sia o meno abitato / abitabile
                    if (!GameSystemDesigner.ActualPlanet.IsHabitable)
                    {
                        spriteBatch.DrawString(font, GameSystemDesigner.ActualPlanet.Name + " non e' abitabile.", new Vector2(15, GraphicSettings.ScreenBounds.Height - 40), Color.White);
                    }
                    else
                    {
                        if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                            spriteBatch.DrawString(font, GameSystemDesigner.ActualPlanet.Name + " e' abitato.", new Vector2(15, GraphicSettings.ScreenBounds.Height - 40), Color.White);
                        else
                            spriteBatch.DrawString(font, GameSystemDesigner.ActualPlanet.Name + " e' abitabile.", new Vector2(15, GraphicSettings.ScreenBounds.Height - 40), Color.White);

                    }
                    pointedButton.Position = new Rectangle(10, hei, pointedButton.ButtonTexture.Width, pointedButton.ButtonTexture.Height);
                    pointedButton.DrawButton(spriteBatch);
                }
            }

            // GUI relativa alla parte di gioco nella galassia
            else if (GameWindow.ActualState == GameWindow.GameState.GALAXY)
            {
                // Scrivi il nome del sistema puntato sopra di esso
                if (GameGalaxyDesigner.PointedSystem != null)
                    spriteBatch.DrawString(font, GameGalaxyDesigner.PointedSystem.Name, new Vector2(Mouse.GetState().Position.ToVector2().X, Mouse.GetState().Position.ToVector2().Y - 20), Color.White);

                //Se l'astronave si trova sopra un sistema solare
                if (GameManager.ActualSystem != null && GameManager.ActualSystem.SystemPosition.X == PlayerShip.Position.X && GameManager.ActualSystem.SystemPosition.Z == PlayerShip.Position.Z)
                {
                    //Mostra il nome nel pulsante
                    pointedButton.Text = GameManager.ActualSystem.Name;
                    int hei = GraphicSettings.ScreenBounds.Height - pointedButton.ButtonTexture.Height - 10;
                    //E se è scoperto anche i pianeti che lo compongono e l'impero padrone
                    if (GameManager.ActualSystem.IsDiscovered)
                    {
                        hei -= 60;
                        spriteBatch.DrawString(font, "Pianeti: " + GameManager.ActualSystem.Planets.Count.ToString(), new Vector2(15, GraphicSettings.ScreenBounds.Height - 60), Color.White);
                        if (GameManager.ActualSystemOwner != null)
                        {
                            spriteBatch.DrawString(font, "Impero: " + GameManager.ActualSystemOwner.EmpireName, new Vector2(15, GraphicSettings.ScreenBounds.Height - 45), Color.White);
                            if (GameManager.ActualSystemOwner != GameManager.PlayerEmpire)
                            {
                                //Mostra felicità
                                spriteBatch.DrawString(font, GameManager.ActualSystemOwner.ShowRelationWithPlayer().GetStatus().ToString(), new Vector2(15, GraphicSettings.ScreenBounds.Height - 30), Color.White);
                                spriteBatch.DrawString(font, GameManager.ActualSystemOwner.ShowRelationWithPlayer().RelationPoints.ToString(), new Vector2(100, GraphicSettings.ScreenBounds.Height - 30), Color.White);
                            }
                        }
                    }
                    pointedButton.Position = new Rectangle(10, hei, pointedButton.ButtonTexture.Width, pointedButton.ButtonTexture.Height);
                    pointedButton.DrawButton(spriteBatch);
                }
            }
            spriteBatch.End();
        }
    }
}
