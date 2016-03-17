using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace GalacticEmpire
{
    class SplashScreenWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        Texture2D logo;
        Texture2D background;
        Texture2D presents;

        Color logoColor;
        Rectangle logoPosition;
        Rectangle presentsPosition;

        int time = 0;

        bool presentationFinished;
        public bool PresentationFinished { get { return presentationFinished; } }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public SplashScreenWindow(Game game) : base(game)
        {
            presentationFinished = false;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(ContentManager content)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            logo = Game.Content.Load<Texture2D>(GUIFolder + "SplashScreen/Logo");
            presents = Game.Content.Load<Texture2D>(GUIFolder + "SplashScreen/Presents");
            background = Game.Content.Load<Texture2D>("Skybox/BackgroundSS");

            Vector2 centerScreen = GraphicSettings.CenterScreen;

            logoPosition = new Rectangle((int)(centerScreen.X - logo.Width / 2), (int)(centerScreen.Y - logo.Height / 2), logo.Width, logo.Height);
            presentsPosition = new Rectangle((int)logoPosition.Width - presents.Width, logoPosition.Height, presents.Width, presents.Height);
            logoColor = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            time++;
            if (time < 255)
            {
                logoColor = new Color(time, time, time, time);
            }
            
            else if (time > 500)
                presentationFinished = true;
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, GraphicSettings.ScreenBounds, Color.White);
           
                spriteBatch.Draw(logo, logoPosition, logoColor);
                spriteBatch.Draw(presents, presentsPosition, logoColor);

            spriteBatch.End();
        }
    }
}
