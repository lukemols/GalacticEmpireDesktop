using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace GalacticEmpire
{
    class GameWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public enum GameState { GALAXY, SYSTEM, PLANET, PAUSED, FINISHED }
        SpriteBatch spriteBatch;

        static GameState lastState;
        static GameState actualState;
        static public GameState ActualState { get { return actualState; } }

        int lastMinuteSave = 0;
        int autosaveIndex = 1;
        int lastClickTime = 0;

        SoundEffect enterPlanet;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public GameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            lastState = GameState.GALAXY;
            actualState = GameState.GALAXY;
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(Game game)
        {
            enterPlanet = game.Content.Load<SoundEffect>("Sounds/Effects/Contact");
            StaticStarModels.LoadModels(game);
            StaticPlanetModels.LoadModels(game);
            PlayerShip.Load(game);

            GameGalaxyDesigner.Load(game);
            GameSystemDesigner.Load(game);

            GameUIDesigner.Load(game);
            GamePlanetUIDesigner.Load(game);
            PauseDesigner.Load(game);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            lastClickTime++;

            if (gameTime.TotalGameTime.Minutes - lastMinuteSave > GameParams.autosaveMinutes && (actualState == GameState.GALAXY || actualState == GameState.SYSTEM))
            {
                lastMinuteSave = gameTime.TotalGameTime.Minutes;
                SaveGameClass.SaveGame("AutoSave" + autosaveIndex++);
                if (autosaveIndex > GameParams.autosaveNumber) autosaveIndex = 1;
            }

            switch(actualState)
            {
                case GameState.PAUSED:
                    switch(PauseDesigner.ActionToPerform())
                    {
                        case PauseDesigner.PauseState.GAME:
                            actualState = lastState;
                            break;
                        case PauseDesigner.PauseState.MENUSAVED:
                            actualState = GameState.FINISHED;
                            break;
                        case PauseDesigner.PauseState.EXITSAVED:
                            Game.Exit();
                            break;
                    }
                    if(GameUIDesigner.ButtonClicked() == "PauseButton" && lastClickTime > 10)
                    {
                        actualState = lastState;
                        lastClickTime = 0;
                    }
                    break;
                case GameState.GALAXY:
                    GameGalaxyDesigner.SetTarget();
                    GameManager.Update(gameTime);
                    if ((Keyboard.GetState().IsKeyDown(Keys.Enter) || GameUIDesigner.ButtonClicked() == "PointedButton") && GameManager.ActualSystem != null && !PlayerShip.IsMoving)
                    {
                        lastClickTime = 0;
                        GameSystemDesigner.EnterSystem();
                        actualState = GameState.SYSTEM;
                    }
                    if (GameUIDesigner.ButtonClicked() == "PauseButton" && lastClickTime > 10)
                    {
                        lastClickTime = 0;
                        actualState = GameState.PAUSED;
                        lastState = GameState.GALAXY;
                        PauseDesigner.State = PauseDesigner.PauseState.PAUSE;
                    }
                    break;
                case GameState.SYSTEM:
                    GameManager.Update(gameTime);
                    GameSystemDesigner.SetTarget();
                    if (GameUIDesigner.ButtonClicked() == "ReturnToGalaxy" || Keyboard.GetState().IsKeyDown(Keys.G))
                    {
                        lastClickTime = 0;
                        actualState = GameState.GALAXY;
                        PlayerShip.SetPosition(GameManager.ActualSystem.SystemPosition);
                    }
                    else if ((Keyboard.GetState().IsKeyDown(Keys.Enter) || GameUIDesigner.ButtonClicked() == "PointedButton")
                        && GameSystemDesigner.ActualPlanet != null && !PlayerShip.IsMoving 
                        && GameSystemDesigner.ActualPlanet.IsHabitable && lastClickTime > 10)
                    {
                        lastClickTime = 0;
                        actualState = GameState.PLANET;
                        GamePlanetUIDesigner.Initialize();
                        GameCommerceManager.EnterPlanet(GameSystemDesigner.ActualPlanet);
                        if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                            GamePlanetUIDesigner.WriteInhabitedButtons();
                        if (GameParams.soundEnabled)
                            enterPlanet.Play();
                    }
                    if (GameUIDesigner.ButtonClicked() == "PauseButton" && lastClickTime > 10)
                    {
                        lastClickTime = 0;
                        actualState = GameState.PAUSED;
                        lastState = GameState.SYSTEM;
                        PauseDesigner.State = PauseDesigner.PauseState.PAUSE;
                    }
                    break;
                case GameState.PLANET:
                    string bt = GamePlanetUIDesigner.ButtonClicked();
                    if (bt != "NONE" && lastClickTime > 10)
                        lastClickTime = 0;
                    else break;

                    if (bt == "Repair")
                    {
                        PlayerShip.Money -= GameCommerceManager.GetRepairPrize();
                        PlayerShip.RestoreLife();
                        GamePlanetUIDesigner.WriteInhabitedButtons();
                    }
                    else if (bt == "Recharge")
                    {
                        PlayerShip.Money -= GameCommerceManager.GetRechargePrize();
                        PlayerShip.RestoreEnergy();
                        GamePlanetUIDesigner.WriteInhabitedButtons();
                    }
                    else if (bt == "CloseButton")
                    {
                        lastClickTime = 0;
                        actualState = GameState.SYSTEM;
                    }
                    break;
            }

                if (Keyboard.GetState().IsKeyDown(Keys.M))
                    actualState = GameState.FINISHED;
            
        }

        /// <summary>
        /// Disegna il menu principale
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            switch (actualState)
            {
                case GameState.PAUSED:
                    if (lastState == GameState.GALAXY)
                        GameGalaxyDesigner.Draw();
                    else
                        GameSystemDesigner.Draw();
                    GameUIDesigner.Draw();
                    PauseDesigner.Draw();
                    break;
                case GameState.GALAXY:
                    GameGalaxyDesigner.Draw();
                    break;
                case GameState.SYSTEM:
                    GameSystemDesigner.Draw();
                    break;
                case GameState.PLANET:
                    GameSystemDesigner.Draw();
                    GamePlanetUIDesigner.Draw(GameSystemDesigner.ActualPlanet);
                    break;
            }

            GameUIDesigner.Draw();
        }

    }

}