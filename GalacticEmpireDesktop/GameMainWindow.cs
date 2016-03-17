using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GalacticEmpire;
using System.Collections.Generic;
using System.IO;

namespace GalacticEmpire
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMainWindow : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        string activeWindow;

        SplashScreenWindow splashScreenWindow;
        MainMenu mainMenu;
        NewGameWindow newGameWindow;
        LoadingWindow loadingWindow;
        GalacticEmpire.GameWindow gameWindow;
        CreditsWindow creditsWindow;
        SettingsWindow settingsWindow;
        LoadGameWindow loadGameWindow;

        List<Song> tracks;
        int songIndex;
        int keyPressedTime;
        
        public GameMainWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            activeWindow = "SplashScreen";
            NameGenerator.LoadFile();
            tracks = new List<Song>();
            keyPressedTime = 0;
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

            splashScreenWindow = new SplashScreenWindow(this);
            mainMenu = new MainMenu(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;

            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.PreferredBackBufferWidth = 1680;
            //graphics.PreferredBackBufferHeight = 1050;
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
            splashScreenWindow.Load(Content);
            mainMenu.Load(Content);

            tracks.Add(Content.Load<Song>(@"Sounds/Music/Myst on the Moor"));
            tracks.Add(Content.Load<Song>("Sounds/Music/Odyssey"));
            tracks.Add(Content.Load<Song>("Sounds/Music/Phantom from Space"));
            songIndex = 0;
            GameParams.musicEnabled = false;
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
                ExitGame();

            keyPressedTime++;
            //Cambia canzone se il player è stoppato o se si preme tab, ma solo se non si è nella splash screen e la musica è abilitata
            if ((MediaPlayer.State == MediaState.Stopped || Keyboard.GetState().IsKeyDown(Keys.Tab) && keyPressedTime > 10)
                && activeWindow != "SplashScreen" && GameParams.musicEnabled == true)
            {
                if (++songIndex == tracks.Count)
                    songIndex = 0;
                MediaPlayer.Play(tracks[songIndex]);
                keyPressedTime = 0;
            }
            if (GameParams.musicEnabled == false)
                MediaPlayer.Pause();
            else if (MediaPlayer.State == MediaState.Paused && GameParams.musicEnabled)
                MediaPlayer.Play(tracks[songIndex]);

            switch (activeWindow)
            {
                case "SplashScreen":
                    splashScreenWindow.Update(gameTime);
                    if (splashScreenWindow.PresentationFinished)
                    {
                        GameParams.musicEnabled = true;
                        MediaPlayer.Play(tracks[songIndex]);
                        activeWindow = "MainMenu";
                    }
                    break;
                case "MainMenu":
                    mainMenu.Update(gameTime);
                    activeWindow = mainMenu.GetNextWindow();
                    if (activeWindow == "ExitButton")
                        ExitGame();
                    CreateNewGameWindow();

                    break;

                case "NewGame":
                    newGameWindow.Update(gameTime);
                    if (newGameWindow.StartLoading)
                    {
                        activeWindow = "Loading";
                        CreateNewGameWindow();
                    }
                    else if(newGameWindow.BackToMenu)
                    {
                        activeWindow = "MainMenu";
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
                case "Credits":
                    creditsWindow.Update(gameTime);
                    if (creditsWindow.BackPressed)
                        activeWindow = "MainMenu";
                    break;
                case "Settings":
                    settingsWindow.Update(gameTime);
                    if (settingsWindow.Finished)
                        activeWindow = "MainMenu";
                    break;
                case "LoadGame":
                    loadGameWindow.Update(gameTime);
                    if (loadGameWindow.BackToMenu)
                        activeWindow = "MainMenu";
                    else if (loadGameWindow.StartLoading)
                    {
                        loadingWindow = new LoadingWindow(this, loadGameWindow.FilePath);
                        loadingWindow.Load(Content);
                        activeWindow = "Loading";
                    }
                    break;

                default:
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
                case "SplashScreen":
                    splashScreenWindow.Draw(gameTime);
                    break;
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
                case "Credits":
                    creditsWindow.Draw(gameTime);
                    break;
                case "Settings":
                    settingsWindow.Draw(gameTime);
                    break;
                case "LoadGame":
                    loadGameWindow.Draw(gameTime);
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
                case "ContinueGame":
                    if(File.Exists("LastSave"))
                    {
                        StreamReader sr = new StreamReader("LastSave");
                        string path = sr.ReadLine();
                        if(File.Exists(path))
                        {
                            loadingWindow = new LoadingWindow(this, path);
                            loadingWindow.Load(Content);
                            activeWindow = "Loading";
                        }
                        sr.Close();
                    }
                    break;
                case "Credits":
                    creditsWindow = new CreditsWindow(this);
                    creditsWindow.Load(this);
                    break;
                case "Settings":
                    settingsWindow = new SettingsWindow(this);
                    settingsWindow.Load(Content);
                    break;
                case "LoadGame":
                    loadGameWindow = new LoadGameWindow(this);
                    loadGameWindow.Load(Content);
                    break;
            }
        }

        public void ExitGame()
        {
            Exit();
        }
    }
}
