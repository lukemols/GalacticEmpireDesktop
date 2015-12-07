using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GalacticEmpire
{
    class GameWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public enum GameState { GALAXY, SYSTEM, PLANET, COMMERCE, PAUSED, SAVING, FINISHED }
        SpriteBatch spriteBatch;

        static GameState actualState;
        static public GameState ActualState { get { return actualState; } }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game"></param>
        public GameWindow(Game game) : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            actualState = GameState.GALAXY;
        }

        /// <summary>
        /// Carica le texture e il font
        /// </summary>
        /// <param name="content"></param>
        public void Load(Game game)
        {
            StaticStarModels.LoadModels(game);
            StaticPlanetModels.LoadModels(game);
            PlayerShip.Load(game);

            GameGalaxyDesigner.Load(game);
            GameSystemDesigner.Load(game);

            GameUIDesigner.Load(game);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            switch(actualState)
            {
                case GameState.PAUSED:
                    break;
                case GameState.GALAXY:
                    GameGalaxyDesigner.SetTarget();
                    GameManager.Update(gameTime);
                    if ((Keyboard.GetState().IsKeyDown(Keys.Enter) || GameUIDesigner.ButtonClicked() == "PointedButton") && GameManager.ActualSystem != null && !PlayerShip.IsMoving)
                    {
                        GameSystemDesigner.EnterSystem();
                        actualState = GameState.SYSTEM;
                    }
                    break;
                case GameState.SYSTEM:
                    GameManager.Update(gameTime);
                    GameSystemDesigner.SetTarget();
                    if (GameUIDesigner.ButtonClicked() == "ReturnToGalaxy" || Keyboard.GetState().IsKeyDown(Keys.G))
                    {
                        actualState = GameState.GALAXY;
                        PlayerShip.SetPosition(GameManager.ActualSystem.SystemPosition);
                    }
                    else if ((Keyboard.GetState().IsKeyDown(Keys.Enter) || GameUIDesigner.ButtonClicked() == "PointedButton") && GameSystemDesigner.ActualPlanet != null && !PlayerShip.IsMoving && GameSystemDesigner.ActualPlanet.IsHabitable)
                    {
                        actualState = GameState.PLANET;
                        GameCommerceManager.EnterPlanet(GameSystemDesigner.ActualPlanet);
                        if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
                            GamePlanetUIDesigner.WriteInhabitedButtons();
                    }
                    break;
                case GameState.PLANET:
                    if (GamePlanetUIDesigner.ButtonClicked() == "Repair")
                    {
                        PlayerShip.Money -= GameCommerceManager.GetRepairPrize();
                        PlayerShip.RestoreLife();
                        GamePlanetUIDesigner.WriteInhabitedButtons();
                    }
                    else if (GamePlanetUIDesigner.ButtonClicked() == "Recharge")
                    {
                        PlayerShip.Money -= GameCommerceManager.GetRechargePrize();
                        PlayerShip.RestoreEnergy();
                        GamePlanetUIDesigner.WriteInhabitedButtons();
                    }
                    else if (GamePlanetUIDesigner.ButtonClicked() == "CloseButton")
                        actualState = GameState.SYSTEM;
                    break;
                case GameState.COMMERCE:
                    if (GamePlanetUIDesigner.ButtonClicked() == "CloseButton")
                        actualState = GameState.SYSTEM;
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
                    break;
                case GameState.GALAXY:
                    GameGalaxyDesigner.Draw();
                    break;
                case GameState.SYSTEM:
                    GameSystemDesigner.Draw();
                    break;
                case GameState.PLANET:
                case GameState.COMMERCE:
                    GameSystemDesigner.Draw();
                    GamePlanetUIDesigner.Draw(GameSystemDesigner.ActualPlanet);
                    break;
                    
            }

            GameUIDesigner.Draw();
        }

    }

}