using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GalacticEmpire
{
    class NewGameWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;

        bool startLoading;
        public bool StartLoading { get { return startLoading; } }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public NewGameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            startLoading = false;
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(ContentManager content)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();
            font = content.Load<SpriteFont>(GUIFolder + "Consolas");
        }

        /// <summary>
        /// Setta le posizioni dei pulsanti e delle stringhe
        /// </summary>
        void SetPositions()
        {

        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                GameParams.empireNumber = 150;
                GameParams.starNumber = 1000;
                GameParams.name = NameGenerator.GetName(4);
                GameParams.religionType = Religion.GetRandomReligion();
                startLoading = true;
            }
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "NEW GAME WINDOW", new Vector2(100, 100), Color.Black);

            spriteBatch.End();
        }
    }
}
