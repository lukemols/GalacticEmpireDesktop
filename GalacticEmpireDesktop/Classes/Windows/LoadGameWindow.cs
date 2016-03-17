using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace GalacticEmpire
{
    class LoadGameWindow : Microsoft.Xna.Framework.DrawableGameComponent
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

        List<string> fileInfo;

        int? actualOptionChoiceNumber;
        string fileFolder;
        string actualChoice;
        string actualChoiceFolder;
        string[] empireNames;
        string[] actualChoiceFiles;

        public string FilePath { get { return actualChoice; } }

        enum Option { SAVE1, SAVE2, SAVE3, SAVE4, SAVE5, NULL };
        Option actualOption;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public LoadGameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            startLoading = false;
            backToMenu = false;
            actualOption = Option.NULL;
            actualChoice = "";
            ObtainSavedFiles();
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
            background = content.Load<Texture2D>(@"Skybox/BackgroundLoad");

            SetPositions();
        }

        void ObtainSavedFiles()
        {
            //Ottieni la cartella Documenti
            var pathWithEnv = @"%USERPROFILE%\Documents";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            //Crea la cartella Galactic Empire e all'interno quella specifica del gioco
            filePath += @"\Galactic Empire\";
            fileFolder = filePath;
            empireNames = Directory.GetDirectories(filePath);
            for (int i = 0; i < empireNames.Length; i++)
                empireNames[i] = Path.GetFileNameWithoutExtension(empireNames[i]);
        }

        void ObtainActualChoiceFile(int gameIndex)
        {
            string folder = fileFolder + empireNames[gameIndex];
            actualChoiceFiles = Directory.GetFiles(folder);
            actualChoiceFolder = folder;
            for (int i = 0; i < actualChoiceFiles.Length; i++)
                actualChoiceFiles[i] = Path.GetFileNameWithoutExtension(actualChoiceFiles[i]);
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

            Vector2 v = font.MeasureString("Scegli gioco salvato");
            Vector2 newBtTxt = new Vector2(v.X + 25, v.Y + 25);
            Rectangle r = new Rectangle(w - (int)newBtTxt.X / 2, h, (int)newBtTxt.X, (int)newBtTxt.Y);
            buttons.Add(new Button(r, "Scegli gioco salvato", "Title"));
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
            buttons.Add(new Button(r, "Save1", "Save1"));
            buttons[3].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)5 / 4 * h), w, h);
            buttons.Add(new Button(r, "Save2", "Save2"));
            buttons[4].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)10 / 4 * h), w, h);
            buttons.Add(new Button(r, "Save3", "Save3"));
            buttons[5].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)15 / 4 * h), w, h);
            buttons.Add(new Button(r, "Save4", "Save4"));
            buttons[6].LoadTextureAndFont(buttonTexture, font);

            r = new Rectangle(10, (int)(hs + (float)20 / 4 * h), w, h);
            buttons.Add(new Button(r, "Save5", "Save5"));
            buttons[7].LoadTextureAndFont(buttonTexture, font);

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

            SetFileToButtons();
            foreach (Button b in buttons)
                b.SetTextPosition();
            foreach (Button b in optionButtons)
            {
                b.SetTextPosition();
                b.IsVisible = false;
            }
        }

        void SetFileToButtons()
        {
            int l = empireNames.Length;
            if (l >= 5)
                l = 5;
            int i = 3, j;
            for(j = 0; j < l; j++)
            {
                buttons[i + j].Text = empireNames[j];
            }
            for (; j < 5; j++)
                buttons[i + j].IsVisible = false;
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
                        case "Save1":
                            ObtainActualChoiceFile(0);
                            actualOption = Option.SAVE1;
                            break;
                        case "Save2":
                            ObtainActualChoiceFile(1);
                            actualOption = Option.SAVE2;
                            break;
                        case "Save3":
                            ObtainActualChoiceFile(2);
                            actualOption = Option.SAVE3;
                            break;
                        case "Save4":
                            ObtainActualChoiceFile(3);
                            actualOption = Option.SAVE4;
                            break;
                        case "Save5":
                            ObtainActualChoiceFile(4);
                            actualOption = Option.SAVE5;
                            break;
                        case "StartGame":
                            startLoading = true;
                            actualOption = Option.NULL;
                            break;
                        case "BackToMenu":
                            backToMenu = true;
                            break;
                    }
                    if (actualOption != Option.NULL)
                    {
                        actualChoice = "";
                        SetOptionButtonsText();
                    }
                    foreach (Button bt in optionButtons)
                        bt.SetTextPosition();
                }
            }
        }

        void SetOptionButtonsText()
        {
            int i;
            for(i = 0; i < actualChoiceFiles.Length; i++)
            {
                string date = File.GetLastAccessTime(actualChoiceFolder + @"\" + actualChoiceFiles[i] + ".ges").ToString();
                optionButtons[i].Text = actualChoiceFiles[i] + "\n" + date;
                optionButtons[i].IsVisible = true;
            }
            for (; i < optionButtons.Count; i++)
                optionButtons[i].IsVisible = false;
        }

        /// <summary>
        /// Metodo che controlla se è stato cliccato un pulsante dei parametri da settare
        /// </summary>
        void ControlOptionClicked()
        {
            int? opt = GetOptionClicked();
            actualOptionChoiceNumber = opt;
            if (opt == null)
                return;
            actualChoice = actualChoiceFolder + @"\" + actualChoiceFiles[(int)opt] + ".ges";
            ReadInfo();
        }

        void ReadInfo()
        {
            if(fileInfo == null)
                fileInfo = new List<string>();
            fileInfo.Clear();
            fileInfo.Add("Impero " + Path.GetFileNameWithoutExtension(actualChoiceFolder));//Nome Impero
            fileInfo.Add("Salvataggio: " + Path.GetFileNameWithoutExtension(actualChoice));//Nome File
            fileInfo.Add(File.GetLastAccessTime(actualChoice).ToString());//Data ultimo salvataggio
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

            if (actualChoice != "")
                for (int i = 0; i < fileInfo.Count; i++)
                {
                    int x = (int)font.MeasureString(fileInfo[i]).X + 10;
                    int y = (int)font.MeasureString(fileInfo[i]).Y + 10;
                    spriteBatch.DrawString(font, fileInfo[i], new Vector2(GraphicSettings.ScreenBounds.Width - x, 100 + i * y), Color.Yellow);
                }

            spriteBatch.End();
        }
    }
}
