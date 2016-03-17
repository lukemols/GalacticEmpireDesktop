using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace GalacticEmpire
{
    class CreditsWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D buttonTexture;
        Texture2D background;
        Texture2D titleTexture;

        Vector2 titlePosition;
        Vector2 textPosition;

        Button backButton;
        bool backPressed;
        public bool BackPressed { get { return backPressed; } }

        string[] text = { "Game Design: Luca Mollo" ,
                          "Programma: Luca Mollo",
                          "Texture e modelli 3D: Luca Mollo",
                          @"Astronave: ysup12 da tf3dm.com",
                          "Musiche: Kevin Mac Leod da incompetech.com",
                          "Effetti sonori da flashkit.com"};
        
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public CreditsWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            backPressed = false;
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();
            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            buttonTexture = game.Content.Load<Texture2D>(GUIFolder + "InGameButton");
            titleTexture = game.Content.Load<Texture2D>(GUIFolder + "TitleLogo");
            background = game.Content.Load<Texture2D>(@"Skybox/BackgroundCredits");

            GraphicsDevice device = game.GraphicsDevice;
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;
            Vector2 buttonDimensions = new Vector2(buttonTexture.Width, buttonTexture.Height);

            string s = "Torna al menu";
            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D txt = new Texture2D(device, (int)font.MeasureString(s).X + 20, (int)font.MeasureString(s).Y + 20);

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(64, 64, 128, 255);
            txt.SetData(data);

            backButton = new Button(new Rectangle(10, Height - (int)buttonDimensions.Y - 10,
                (int)buttonDimensions.X, (int)buttonDimensions.Y), "Torna al menu", "MenuButton");
            backButton.LoadTextureAndFont(txt, font);
            backButton.SetTextPosition();
            backButton.SetTextColor(Color.Yellow);

            titlePosition = new Vector2((Width - titleTexture.Width) / 2, Height);
            textPosition = new Vector2(0, titleTexture.Height + Height);
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if(titlePosition.Y > 0)
                titlePosition.Y -= 1;
            if(textPosition.Y > titleTexture.Height)
                textPosition.Y -= 1;
            backPressed = backButton.WasPressed();
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            backButton.DrawButton(spriteBatch);

            int w = (int)GraphicSettings.CenterScreen.X;
            int i = 0;
            foreach (string s in text)
            {
                int x = (int)font.MeasureString(s).X;
                spriteBatch.DrawString(font, s, new Vector2(w - x / 2, textPosition.Y + 10 + i * (int)font.MeasureString(s).Y), Color.Yellow);
                i++;
            }
            spriteBatch.Draw(titleTexture, titlePosition, Color.White);

            spriteBatch.End();
        }
    }
}
