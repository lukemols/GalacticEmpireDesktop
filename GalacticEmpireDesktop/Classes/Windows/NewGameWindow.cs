﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace GalacticEmpire
{
    class NewGameWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D buttonTexture;
        Texture2D background;

        bool startLoading;
        public bool StartLoading { get { return startLoading; } }

        bool backToMenu;
        public bool BackToMenu { get { return backToMenu; } }

        List<Button> buttons;
        List<Button> optionButtons;

        enum Option { DIMENSION, EMPIRES, RELIGION, DIFFICULTY, NULL};
        Option actualOption;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public NewGameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            startLoading = false;
            backToMenu = false;

            actualOption = Option.NULL;
            GameParams.empireNumber = 150;
            GameParams.starNumber = 1000;
            GameParams.name = NameGenerator.GetName(4);
            GameParams.religionType = Religion.GetRandomReligion();
            GameParams.gameDifficulty = GameParams.Difficulty.EASY;
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(ContentManager content)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();
            font = content.Load<SpriteFont>(GUIFolder + "Consolas");
            buttonTexture = content.Load<Texture2D>(GUIFolder + "InGameButton");
            background = content.Load<Texture2D>(@"Skybox/BackgroundNG");

            SetPositions();
        }

        /// <summary>
        /// Setta le posizioni dei pulsanti e delle stringhe
        /// </summary>
        void SetPositions()
        {
            buttons = new List<Button>();
            optionButtons = new List<Button>();

            int w = (int)GraphicSettings.CenterScreen.X;
            int h = 10;

            Vector2 v = font.MeasureString("Nuovo gioco");
            Vector2 newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            Rectangle r = new Rectangle(w - (int)newBtTxt.X / 2, h, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Nuovo Gioco", "Title"));
            buttons[0].LoadTextureAndFont(buttonTexture, font);

            w = GraphicSettings.ScreenBounds.Width;
            h = GraphicSettings.ScreenBounds.Height;
            v = font.MeasureString("Inizia gioco");
            newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            r = new Rectangle(w - (int)newBtTxt.X - 10, h - (int)newBtTxt.Y - 10, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Inizia Gioco", "StartGame"));
            buttons[1].LoadTextureAndFont(buttonTexture, font);

            v = font.MeasureString("Torna al menu");
            newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            r = new Rectangle(10, h - (int)newBtTxt.Y - 10, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Torna al menu", "BackToMenu"));
            buttons[2].LoadTextureAndFont(buttonTexture, font);

            int hs = buttonTexture.Height + 20;
            h = (h - hs) / 6;
            if (h > 100)
                h = 100;
            w = w / 3;
            if (w > 350)
                w = 350;
            ///PULSANTI DEL TIPO DI OPZIONE
            r = new Rectangle(10, hs, w, h);
            buttons.Add(new Button(r, "Dimensioni galassia\nGrande", "Dimensions"));
            buttons[3].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)5 /4 * h), w, h);
            buttons.Add(new Button(r, "Numero di imperi\nTanti", "Empires"));
            buttons[4].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)10 /4 * h), w, h);
            buttons.Add(new Button(r, "Religione\nCasuale", "Religion"));
            buttons[5].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)15/4 * h), w, h);
            buttons.Add(new Button(r, "Difficoltà\nFacile", "Difficulty"));
            buttons[6].LoadTextureAndFont(buttonTexture, font);

            ///PULSANTI PER LA SCELTA
            int ws = w + 20;
            r = new Rectangle(ws, hs, w, h);
            optionButtons.Add(new Button(r, "OPT0", "Option0"));
            optionButtons[0].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(ws, (int)(hs + (float)5 / 4 * h), w, h);
            optionButtons.Add(new Button(r, "OPT1", "Option1"));
            optionButtons[1].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(ws, (int)(hs + (float)10 / 4 * h), w, h);
            optionButtons.Add(new Button(r, "OPT2", "Option2"));
            optionButtons[2].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(ws, (int)(hs + (float)15 / 4 * h), w, h);
            optionButtons.Add(new Button(r, "OPT3", "Option3"));
            optionButtons[3].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(ws, (int)(hs + (float)20 / 4 * h), w, h);
            optionButtons.Add(new Button(r, "OPT4", "Option4"));
            optionButtons[4].LoadTextureAndFont(buttonTexture, font);


            foreach (Button b in buttons)
                b.SetTextPosition();
            foreach (Button b in optionButtons)
            {
                b.SetTextPosition();
                b.IsVisible = false;
            }
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            ControlButtonClicked();
            if (actualOption != Option.NULL)
                ControlOptionClicked();
        }

        /// <summary>
        /// Metodo che controlla se è stato cliccato un pulsante delle opzioni
        /// </summary>
        void ControlButtonClicked()
        {
            foreach (Button b in buttons)
            {
                if (b.WasPressed())
                {
                    switch (b.Type)
                    {
                        case "Dimensions":
                            optionButtons[0].Text = "Piccola";
                            optionButtons[1].Text = "Media";
                            optionButtons[2].Text = "Grande";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = true;
                            optionButtons[3].IsVisible = false;
                            optionButtons[4].IsVisible = false;
                            actualOption = Option.DIMENSION;
                            break;
                        case "Empires":
                            optionButtons[0].Text = "Pochi";
                            optionButtons[1].Text = "Nella norma";
                            optionButtons[2].Text = "Tanti";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = true;
                            optionButtons[3].IsVisible = false;
                            optionButtons[4].IsVisible = false;
                            actualOption = Option.EMPIRES;
                            break;
                        case "Religion":
                            optionButtons[0].Text = "Casuale";
                            optionButtons[1].Text = "Ateo";
                            optionButtons[2].Text = "Atom";
                            optionButtons[3].Text = "Bless";
                            optionButtons[4].Text = "Curser";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = true;
                            optionButtons[3].IsVisible = true;
                            optionButtons[4].IsVisible = true;
                            actualOption = Option.RELIGION;
                            break;
                        case "Difficulty":
                            optionButtons[0].Text = "Facile";
                            optionButtons[1].Text = "Normale";
                            optionButtons[2].Text = "Difficile";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = true;
                            optionButtons[3].IsVisible = false;
                            optionButtons[4].IsVisible = false;
                            actualOption = Option.DIFFICULTY;
                            break;
                        case "StartGame":
                            startLoading = true;
                            break;
                        case "BackToMenu":
                            backToMenu = true;
                            break;
                    }

                    foreach (Button bt in optionButtons)
                        bt.SetTextPosition();
                }
            }
        }

        /// <summary>
        /// Metodo che controlla se è stato cliccato un pulsante dei parametri da settare
        /// </summary>
        void ControlOptionClicked()
        {
            int? opt = GetOptionClicked();
            if (opt == null)
                return;
            switch(actualOption)
            {
                case Option.DIFFICULTY:
                    if (opt == 0)
                        GameParams.gameDifficulty = GameParams.Difficulty.EASY;
                    else if (opt == 1)
                        GameParams.gameDifficulty = GameParams.Difficulty.NORMAL;
                    else if (opt == 2)
                        GameParams.gameDifficulty = GameParams.Difficulty.HARD;
                    buttons[6].Text = "Difficoltà\n" + optionButtons[(int)opt].Text;
                    buttons[6].SetTextPosition();
                    break;
                case Option.DIMENSION:
                    if (opt == 0)
                        GameParams.starNumber = 15;
                    else if (opt == 1)
                        GameParams.starNumber = 750;
                    else if (opt == 2)
                        GameParams.starNumber = 1000;
                    buttons[3].Text = "Dimensioni galassia\n" + optionButtons[(int)opt].Text;
                    buttons[3].SetTextPosition();
                    break;
                case Option.EMPIRES:
                    if (opt == 0)
                        GameParams.empireNumber = 75;
                    else if (opt == 1)
                        GameParams.empireNumber = 125;
                    else if (opt == 2)
                        GameParams.empireNumber = 150;
                    buttons[4].Text = "Numero di Imperi\n" + optionButtons[(int)opt].Text;
                    buttons[4].SetTextPosition();
                    break;
                case Option.RELIGION:
                    if (opt == 0)
                        GameParams.religionType = Religion.GetRandomReligion();
                    else if (opt == 1)
                        GameParams.religionType = Religion.ReligionType.ATEO;
                    else if (opt == 2)
                        GameParams.religionType = Religion.ReligionType.ATOM;
                    else if (opt == 3)
                        GameParams.religionType = Religion.ReligionType.BLESS;
                    else if (opt == 4)
                        GameParams.religionType = Religion.ReligionType.CURSER;
                    buttons[5].Text = "Religione\n" + optionButtons[(int)opt].Text;
                    buttons[5].SetTextPosition();
                    break;
            }
            foreach (Button b in optionButtons)
                b.IsVisible = false;
        }

        /// <summary>
        /// Metodo che ritorna il numero del pulsante premuto
        /// </summary>
        /// <returns>Numero del pulsante (può essere null)</returns>
        int? GetOptionClicked()
        {
            int? option = null;
            for(int i = 0; i < optionButtons.Count; i++)
            {
                if (optionButtons[i].WasPressed())
                {
                    option = i;
                }
            }
            return option;
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            foreach (Button b in buttons)
                b.DrawButton(spriteBatch);
            foreach (Button b in optionButtons)
                b.DrawButton(spriteBatch);

            spriteBatch.End();
        }
    }
}
