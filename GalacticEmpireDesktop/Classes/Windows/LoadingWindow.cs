using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GalacticEmpire
{
    class LoadingWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;

        bool isLoading;
        public bool IsLoading { get { return isLoading; } }

        delegate void NewGameDelegate();
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public LoadingWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            isLoading = true;
            Initialize();
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
        /// Inizializza il caricamento
        /// </summary>
        public override void Initialize()
        {
            NewGameDelegate dlgt = new NewGameDelegate(GameManager.NewGame);

            System.AsyncCallback cb = new System.AsyncCallback(LoadComplete);

            System.IAsyncResult ar = dlgt.BeginInvoke(cb, dlgt);
        }

        void LoadComplete(System.IAsyncResult ar)
        {
            NewGameDelegate dlgt = (NewGameDelegate)ar.AsyncState;

            dlgt.EndInvoke(ar);

            isLoading = false;
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if(isLoading)
            spriteBatch.DrawString(font, "LOADING", new Vector2(100, 100), Color.Black);
            else

                spriteBatch.DrawString(font, "LOADING Complete", new Vector2(100, 100), Color.Black);

            spriteBatch.End();
        }
    }
}
