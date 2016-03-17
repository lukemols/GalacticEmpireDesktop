using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace GalacticEmpire
{
    class SettingsWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D buttonTexture;
        Texture2D background;

        bool finished;
        public bool Finished { get { return finished; } }

        List<Button> buttons;
        List<Button> optionButtons;

        enum Option { MUSIC, SOUND, NULL };
        Option actualOption;

        bool music, sounds;


        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public SettingsWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            finished = false;
            music = GameParams.musicEnabled;
            sounds = GameParams.soundEnabled;

            actualOption = Option.NULL;
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
            background = content.Load<Texture2D>(@"Skybox/BackgroundSettings");

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

            Vector2 v = font.MeasureString("Impostazioni");
            Vector2 newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            Rectangle r = new Rectangle(w - (int)newBtTxt.X / 2, h, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Impostazioni", "Title"));
            buttons[0].LoadTextureAndFont(buttonTexture, font);

            w = GraphicSettings.ScreenBounds.Width;
            h = GraphicSettings.ScreenBounds.Height;
            v = font.MeasureString("Accetta modifiche");
            newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            r = new Rectangle(w - (int)newBtTxt.X - 10, h - (int)newBtTxt.Y - 10, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Accetta modifiche", "AcceptButton"));
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
            string s = "Musica\n";
            if (music)
                s += "SI";
            else
                s += "NO";
            buttons.Add(new Button(r, s, "Music"));
            buttons[3].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)5 / 4 * h), w, h);
            s = "Suoni\n";
            if (sounds)
                s += "SI";
            else
                s += "NO";
            buttons.Add(new Button(r, s, "Sounds"));
            buttons[4].LoadTextureAndFont(buttonTexture, font);

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
                        case "Music":
                            optionButtons[0].Text = "SI";
                            optionButtons[1].Text = "NO";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = false;
                            optionButtons[3].IsVisible = false;
                            optionButtons[4].IsVisible = false;
                            actualOption = Option.MUSIC;
                            break;
                        case "Sounds":
                            optionButtons[0].Text = "SI";
                            optionButtons[1].Text = "NO";

                            optionButtons[0].IsVisible = true;
                            optionButtons[1].IsVisible = true;
                            optionButtons[2].IsVisible = false;
                            optionButtons[3].IsVisible = false;
                            optionButtons[4].IsVisible = false;
                            actualOption = Option.SOUND;
                            break;
                        case "AcceptButton":
                            GameParams.musicEnabled = music;
                            GameParams.soundEnabled = sounds;
                            finished = true;
                            break;
                        case "BackToMenu":
                            finished = true;
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
            switch (actualOption)
            {
                case Option.MUSIC:
                    if (opt == 0)
                        music = true;
                    else if (opt == 1)
                        music = false;
                    buttons[3].Text = "Musica\n" + optionButtons[(int)opt].Text;
                    buttons[3].SetTextPosition();
                    break;
                case Option.SOUND:
                    if (opt == 0)
                        sounds = true;
                    else if (opt == 1)
                        sounds = false;
                    buttons[4].Text = "Suoni\n" + optionButtons[(int)opt].Text;
                    buttons[4].SetTextPosition();
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
            for (int i = 0; i < optionButtons.Count; i++)
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
