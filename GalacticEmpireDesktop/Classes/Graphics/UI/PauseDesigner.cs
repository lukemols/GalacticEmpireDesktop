using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    static class PauseDesigner
    {
        static SpriteFont font;
        static List<Button> OnScreenButtons;
        static List<Button> ChoiceButtons;
        static Button SavedFileButton;
        static SpriteBatch spriteBatch;

        static GraphicsDevice device;

        public enum PauseState { PAUSE, GAME, SAVED, MENUASK, MENUSAVED, EXITASK, EXITSAVED};

        static PauseState state = PauseState.PAUSE;
        static public PauseState State { set { if (value == PauseState.PAUSE) state = value; } }
        static PauseState previousState = PauseState.PAUSE;

        static bool askForExit = false;
        static bool askForMenu = false;
        static bool fileSavedFeedback = false;

        static int lastClickTime = 0;

        static public void Load(Game game)
        {
            state = PauseState.PAUSE; lastClickTime = 0;
            string GUIFolder = GraphicSettings.GetGUIFolder();

            OnScreenButtons = new List<Button>();
            ChoiceButtons = new List<Button>();
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");

            device = game.GraphicsDevice;
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;
            Vector2 buttonDimensions = new Vector2(Width / 6, Height / 12);

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(64, 64, 128, 128);
            txt.SetData(data);

            OnScreenButtons.Add(new Button(new Rectangle(), "Continua gioco", "ContinueButton"));
            OnScreenButtons[0].LoadTexture(txt);
            OnScreenButtons.Add(new Button(new Rectangle(), "Salva gioco", "SaveButton"));
            OnScreenButtons[1].LoadTexture(txt);
            OnScreenButtons.Add(new Button(new Rectangle(), "Torna al menu", "MenuButton"));
            OnScreenButtons[2].LoadTexture(txt);
            OnScreenButtons.Add(new Button(new Rectangle(), "Torna a Windows", "ExitButton"));
            OnScreenButtons[3].LoadTexture(txt);

            buttonDimensions = new Vector2(Width / 10, Height / 16);

            data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            txt = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(64, 64, 128, 128);
            txt.SetData(data);

            ChoiceButtons.Add(new Button(new Rectangle(), "Salva ed esci", "YesButton"));
            ChoiceButtons[0].LoadTexture(txt);
            ChoiceButtons.Add(new Button(new Rectangle(), "Esci senza salvare", "NoButton"));
            ChoiceButtons[1].LoadTexture(txt);
            ChoiceButtons.Add(new Button(new Rectangle(), "Annulla", "CancelButton"));
            ChoiceButtons[2].LoadTexture(txt);

            SavedFileButton = new Button(new Rectangle(), "Ok", "SavedFileButton");
            SavedFileButton.LoadTexture(txt);
            SetButtonPositions();
        }

        static private void SetButtonPositions()
        {
            int height = GraphicSettings.ScreenBounds.Height;
            int width = GraphicSettings.ScreenBounds.Width;
            Vector2 center = GraphicSettings.CenterScreen;

            int w = OnScreenButtons[0].ButtonTexture.Width;
            int h = OnScreenButtons[0].ButtonTexture.Height;
            int i = 0;
            int space = height / 6;
            foreach (Button b in OnScreenButtons)
            {
                b.Position = new Rectangle((int)center.X - w / 2, (int)(space + i * h * 1.2f), w, h);
                i++;
            }
            w = ChoiceButtons[0].ButtonTexture.Width;
            h = ChoiceButtons[0].ButtonTexture.Height;
            int s = 25;
            int x = (int)center.X - (w / 2 + w + s);
            i = 0;
            foreach (Button b in ChoiceButtons)
            {
                b.Position = new Rectangle(x + i * w + i * s, (int)center.Y, w, h);
                i++;
            }
            SavedFileButton.Position = new Rectangle((int)center.X - w / 2, (int)center.Y, w, h);
        }

        static string ButtonClicked()
        {
            string button = "NONE";
            if(state == PauseState.PAUSE)
            foreach (Button b in OnScreenButtons)
            {
                if (b.WasPressed())
                    button = b.Type;
            }
            else if(state == PauseState.MENUASK || state == PauseState.EXITASK)
            foreach (Button b in ChoiceButtons)
                if (b.WasPressed())
                    button = b.Type;
            if (state == PauseState.SAVED && SavedFileButton.WasPressed())
                button = SavedFileButton.Type;
            return button;
        }

        static public PauseState ActionToPerform()
        {
            lastClickTime++;
            if (lastClickTime < 10)
                return PauseState.PAUSE;
            string button = ButtonClicked();
            if (button != "NONE")
                lastClickTime = 0;
            switch (button)
            {
                case "ContinueButton":
                    state = PauseState.GAME;
                    break;
                case "SaveButton":
                    SaveGameClass.SaveGame();
                    state = PauseState.SAVED;
                    previousState = PauseState.PAUSE;
                    break;
                case "MenuButton":
                    state = PauseState.MENUASK;
                    break;
                case "ExitButton":
                    state = PauseState.EXITASK;
                    break;
                case "YesButton":
                    previousState = state;
                    SaveGameClass.SaveGame();
                    state = PauseState.SAVED;
                    break;
                case "NoButton":
                    if (state == PauseState.EXITASK)
                        state = PauseState.EXITSAVED;
                    else if (state == PauseState.MENUASK)
                        state = PauseState.MENUSAVED;
                    break;
                case "CancelButton":
                    state = PauseState.PAUSE;
                    break;
                case "SavedFileButton":
                    if (previousState == PauseState.EXITASK)
                        state = PauseState.EXITSAVED;
                    else if (previousState == PauseState.MENUASK)
                        state = PauseState.MENUSAVED;
                    else
                        state = PauseState.PAUSE;
                    break;
            }
            return state;
        }

        static public void Draw()
        {
            spriteBatch.Begin();

            //Disegna i pulsanti sullo schermo
            if (state == PauseState.PAUSE)
                foreach (Button b in OnScreenButtons)
                    b.DrawButton(spriteBatch);
            else if (state == PauseState.MENUASK || state == PauseState.EXITASK)
            {
                string txt = "Vuoi prima salvare il gioco?";
                int xt = (int)font.MeasureString(txt).X;
                int yt = (int)font.MeasureString(txt).Y;
                int cx = (int)GraphicSettings.CenterScreen.X;
                int cy = (int)GraphicSettings.CenterScreen.Y;
                spriteBatch.DrawString(font, txt, new Vector2(cx - xt / 2, cy - yt - 10), Color.White);
                foreach (Button b in ChoiceButtons)
                    b.DrawButton(spriteBatch);
            }
            else if(state == PauseState.SAVED || state == PauseState.EXITSAVED || state == PauseState.MENUSAVED)
            {
                string txt = "Ho salvato il gioco.";
                int xt = (int)font.MeasureString(txt).X;
                int yt = (int)font.MeasureString(txt).Y;
                int cx = (int)GraphicSettings.CenterScreen.X;
                int cy = (int)GraphicSettings.CenterScreen.Y;
                spriteBatch.DrawString(font, txt, new Vector2(cx - xt / 2, cy - yt - 10), Color.White);
                SavedFileButton.DrawButton(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
