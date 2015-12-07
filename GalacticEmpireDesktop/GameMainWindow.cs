using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GalacticEmpire;

namespace GalacticEmpireDesktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMainWindow : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        string activeWindow;

        MainMenu mainMenu;
        NewGameWindow newGameWindow;
        LoadingWindow loadingWindow;
        GalacticEmpire.GameWindow gameWindow;

        public GameMainWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            activeWindow = "MainMenu";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Aggiungi la sprite batch ai servizi cosicché le classi possano usare la stessa istanza
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            mainMenu = new MainMenu(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;

            IsMouseVisible = true;
            graphics.ApplyChanges();

            GraphicSettings.ScreenBoundsChanged(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mainMenu.Load(this.Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (activeWindow)
            {
                case "MainMenu":
                    mainMenu.Update(gameTime);

                    activeWindow = mainMenu.GetNextWindow();
                    CreateNewGameWindow();

                    break;

                case "NewGame":
                    newGameWindow.Update(gameTime);
                    if (newGameWindow.StartLoading)
                    {
                        activeWindow = "Loading";
                        CreateNewGameWindow();
                    }
                    break;

                case "Loading":
                    loadingWindow.Update(gameTime);
                    if (!loadingWindow.IsLoading)
                    {
                        activeWindow = "Game";
                        CreateNewGameWindow();
                    }
                    break;
                case "Game":
                    gameWindow.Update(gameTime);
                    if (GalacticEmpire.GameWindow.ActualState == GalacticEmpire.GameWindow.GameState.FINISHED)
                        activeWindow = "MainMenu";
                    break;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (activeWindow)
            {
                case "MainMenu":
                    mainMenu.Draw(gameTime);
                    break;
                case "NewGame":
                    newGameWindow.Draw(gameTime);
                    break;
                case "Loading":
                    loadingWindow.Draw(gameTime);
                    break;
                case "Game":
                    gameWindow.Draw(gameTime);
                    break;


            }

            base.Draw(gameTime);
        }

        void CreateNewGameWindow()
        {
            switch (activeWindow)
            {
                case "NewGame":
                    newGameWindow = new NewGameWindow(this);
                    newGameWindow.Load(Content);
                    break;
                case "Loading":
                    loadingWindow = new LoadingWindow(this);
                    loadingWindow.Load(Content);
                    break;
                case "Game":
                    gameWindow = new GalacticEmpire.GameWindow(this);
                    gameWindow.Load(this);
                    break;
            }

        }
    }
}
