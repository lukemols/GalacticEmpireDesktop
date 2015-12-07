using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;


namespace GalacticEmpire
{
    class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        SpriteFont font;

        Skybox skybox;
        BasicEffect effect;
        
        Camera galaxyCamera;
        int zoom;

        Texture2D title;
        Vector2 titlePosition;
        float titleZoom;

        List<Button> buttons;

        int menuTime = 0;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public MainMenu(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(ContentManager content)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();
            font = content.Load<SpriteFont>(GUIFolder + "Consolas");

            Model skyboxModel = Game.Content.Load<Model>("Skybox/Skybox");
            skybox = new Skybox(Game.GraphicsDevice, skyboxModel);
            effect = new BasicEffect(Game.GraphicsDevice);

            title = Game.Content.Load<Texture2D>(GUIFolder + "TitleLogo");
            titlePosition = new Vector2(GraphicSettings.CenterScreen.X - title.Width / 2, 50);
            titleZoom = 0.01f;

            CreateInterface(GUIFolder);
        }

        /// <summary>
        /// Setta le posizioni dei pulsanti e delle stringhe
        /// </summary>
        void CreateInterface(string GUIFolder)
        {
            Vector3 cameraPosition = new Vector3(0, 200, 500);
            galaxyCamera = new Camera(cameraPosition, new Vector3(0, 0, 0), 500, MathHelper.PiOver2);
            zoom = 0;
            int w = GraphicSettings.ScreenBounds.Width / 2;
            int h = 3 * GraphicSettings.ScreenBounds.Height / 4;

            buttons = new List<Button>();

            Texture2D buttonTexture = Game.Content.Load<Texture2D>(GUIFolder + "MM-CE");
            int wb = buttonTexture.Width / 2;
            int hb = buttonTexture.Height / 2;
            buttons.Add(new Button(new Rectangle(w - wb, h - hb, buttonTexture.Width, buttonTexture.Height), "Nuovo gioco", "NewGame"));
            buttons[0].LoadTextureAndFont(buttonTexture, font);

            buttonTexture = Game.Content.Load<Texture2D>(GUIFolder + "MM-NE");
            buttons.Add(new Button(new Rectangle(w + wb, h - hb - buttonTexture.Height, buttonTexture.Width, buttonTexture.Height), "Continua gioco", "ContinueGame"));
            buttons[1].LoadTextureAndFont(buttonTexture, font);

            buttonTexture = Game.Content.Load<Texture2D>(GUIFolder + "MM-NW");
            buttons.Add(new Button(new Rectangle(w - wb - buttonTexture.Width, h - hb - buttonTexture.Height, buttonTexture.Width, buttonTexture.Height), "Carica gioco", "LoadGame"));
            buttons[2].LoadTextureAndFont(buttonTexture, font);

            buttonTexture = Game.Content.Load<Texture2D>(GUIFolder + "MM-SE");
            buttons.Add(new Button(new Rectangle(w + wb, h + hb, buttonTexture.Width, buttonTexture.Height), "Crediti", "Credits"));
            buttons[3].LoadTextureAndFont(buttonTexture, font);

            buttonTexture = Game.Content.Load<Texture2D>(GUIFolder + "MM-SW");
            buttons.Add(new Button(new Rectangle(w - wb - buttonTexture.Width, h + hb, buttonTexture.Width, buttonTexture.Height), "Impostazioni", "Settings"));
            buttons[4].LoadTextureAndFont(buttonTexture, font);

            foreach (Button b in buttons)
                b.SetTextPosition();
        }

        /// <summary>
        /// Aggiorna la freccina
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {            
            if (zoom < 100)
                zoom += 1;
            else
                zoom = 0;

            if (titleZoom < 1)
                titleZoom += 0.01f;

            galaxyCamera.AutomaticUpdate(0.005f, zoom);
            menuTime++;
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            skybox.Draw(galaxyCamera.CameraPosition, galaxyCamera.CameraTarget);

            //spriteBatch.DrawString(font, "Premi invio per nuovo gioco", new Vector2(100, 100), Color.Yellow);
            spriteBatch.Draw(title, titlePosition, null, Color.White, 0f, new Vector2(), titleZoom, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, GraphicSettings.GetGUIFolder(), new Vector2(0, 0), Color.White);

            if (titleZoom >= 1)
                foreach (Button b in buttons)
                    b.DrawButton(spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Metodo che ritorna la prossima finestra da mostrare
        /// </summary>
        /// <returns>Stringa con il nome della finestra</returns>
        public string GetNextWindow()
        {
            string next = "MainMenu";
            if (menuTime < 10)
                return next;
            
            foreach (Button b in buttons)
            {
                if (b.WasPressed())
                {
                    next = b.Type;
                    break;
                }
            }

            if (next != "MainMenu")
                menuTime = 0;
            return next;
        }

    }

}
