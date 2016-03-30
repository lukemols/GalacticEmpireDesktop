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
        public enum GameState { GALAXY, SYSTEM, PLANET, PAUSED, FINISHED, TUTORIAL }
        SpriteBatch spriteBatch;

        static GameState lastState;
        static GameState actualState;
        static public GameState ActualState { get { return actualState; } }

        int lastMinuteSave = 0;
        int autosaveIndex = 1;
        int lastClickTime = 0;

        TutorialUIDesigner tutorial;

        SoundEffect enterPlanet;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public GameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            lastState = GameState.GALAXY;
            //actualState = GameState.GALAXY;
            actualState = GameState.TUTORIAL;
            tutorial = new TutorialUIDesigner();
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
            GamePlanetDisabitedUIDesigner.Load(game);
            PauseDesigner.Load(game);
            tutorial.Load(game.Content);
        }

        static public void SetState(GameState state)
        {
            if (state == GameState.GALAXY && GameManager.ActualSystem != null)
            {
                lastState = actualState;
                actualState = state;
                PlayerShip.SetPosition(GameManager.ActualSystem.SystemPosition);
            }
            else if (state == GameState.SYSTEM && GameManager.ActualSystem != null)
            {
                lastState = actualState;
                actualState = state;
                GameSystemDesigner.EnterSystem();
            }
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
                case GameState.TUTORIAL:
                    tutorial.Update();
                    if (tutorial.IsFinished)
                        actualState = GameState.GALAXY;
                    break;
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
                        if(GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                        {
                            GamePlanetUIDesigner.Initialize();
                            GameCommerceManager.EnterPlanet(GameSystemDesigner.ActualPlanet);
                            GamePlanetUIDesigner.WriteInhabitedButtons();
                        }
                        else
                        {
                            GamePlanetDisabitedUIDesigner.Initialize();
                        }
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
                    string bt = "NONE";
                    if(GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                    {
                        bt = GamePlanetUIDesigner.ButtonClicked();
                        if (bt == "Repair")
                        {
                            int prize = GameCommerceManager.GetRepairPrize();
                            if (PlayerShip.Money >= prize)
                            {
                                PlayerShip.Money -= prize;
                                PlayerShip.RestoreLife();
                                GamePlanetUIDesigner.WriteInhabitedButtons();
                            }
                        }
                        else if (bt == "Recharge")
                        {
                            int prize = GameCommerceManager.GetRechargePrize();
                            if (PlayerShip.Money >= prize)
                            {
                                PlayerShip.Money -= prize;
                                PlayerShip.RestoreEnergy();
                                GamePlanetUIDesigner.WriteInhabitedButtons();
                            }
                        }
                    }
                    else
                    {
                        bt = GamePlanetDisabitedUIDesigner.ButtonClicked();
                    }
                    if (bt != "NONE" && lastClickTime > 10)
                    {
                        lastClickTime = 0;
                        if (bt == "CloseButton")
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
                case GameState.TUTORIAL:
                    tutorial.Draw(spriteBatch);
                    break;
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
                    if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                        GamePlanetUIDesigner.Draw(GameSystemDesigner.ActualPlanet);
                    else
                        GamePlanetDisabitedUIDesigner.Draw(GameSystemDesigner.ActualPlanet);
                    break;
            }
            if(actualState != GameState.TUTORIAL)
            GameUIDesigner.Draw();
        }

    }

}