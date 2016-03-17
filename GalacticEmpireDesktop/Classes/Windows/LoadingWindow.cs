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
        Texture2D background;
        Texture2D loadingSprite;

        bool isLoading;
        public bool IsLoading { get { return isLoading; } }

        int i;
        bool loadGame;
        string filePath;

        delegate void NewGameDelegate();
        delegate void LoadGameDelegate(string filePath);
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public LoadingWindow(Game game, string filePath = "") : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            isLoading = true;
            i = 0;
            if (filePath == "")
                loadGame = false;
            else
                loadGame = true;
            this.filePath = filePath;
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
            background = content.Load<Texture2D>("Skybox/BackgroundLG");
            loadingSprite = content.Load<Texture2D>(GUIFolder + "Money");
        }

        /// <summary>
        /// Inizializza il caricamento
        /// </summary>
        public override void Initialize()
        {
            if(loadGame)
            {
                LoadGameDelegate dlgt = new LoadGameDelegate(GameManager.LoadGame);
                System.AsyncCallback cb = new System.AsyncCallback(LoadGameComplete);
                System.IAsyncResult ar = dlgt.BeginInvoke(filePath, cb, dlgt);
            }

            else
            {

#if WINDOWS
                NewGameDelegate dlgt = new NewGameDelegate(GameManager.NewGame);
                System.AsyncCallback cb = new System.AsyncCallback(NewGameComplete);
                System.IAsyncResult ar = dlgt.BeginInvoke(cb, dlgt);
#endif
#if WINDOWS_PHONE_APP
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
               Windows.UI.Core.CoreDispatcherPriority.Normal,
               () => {
                   GameManager.NewGame();
                   isLoading = false;
              });
#endif
            }
        }

        void NewGameComplete(System.IAsyncResult ar)
        {
            NewGameDelegate dlgt = (NewGameDelegate)ar.AsyncState;

            dlgt.EndInvoke(ar);

            isLoading = false;
        }
        void LoadGameComplete(System.IAsyncResult ar)
        {
            LoadGameDelegate dlgt = (LoadGameDelegate)ar.AsyncState;

            dlgt.EndInvoke(ar);

            isLoading = false;
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            i++;
            if (i > 90)
                i = 0;
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            Vector2 center = GraphicSettings.CenterScreen;
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            string txt = "CARICAMENTO IN CORSO...";
            spriteBatch.DrawString(font, txt, new Vector2(center.X - font.MeasureString(txt).X, 25), Color.Yellow);

            switch(i / 30)
            {
                case 0:
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X - loadingSprite.Width * 2.5f, center.Y * 1.8f), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X - loadingSprite.Width * 2.5f, center.Y * 1.8f), Color.White);
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X - loadingSprite.Width / 2, center.Y * 1.8f), Color.White);
                    break;
                case 2:
                default:
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X - loadingSprite.Width / 2, center.Y * 1.8f), Color.White);
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X - loadingSprite.Width * 2.5f, center.Y * 1.8f), Color.White);
                    spriteBatch.Draw(loadingSprite, new Vector2(center.X + loadingSprite.Width * 1.5f, center.Y * 1.8f), Color.White);
                    break;
            }

            spriteBatch.End();
        }
    }
}
