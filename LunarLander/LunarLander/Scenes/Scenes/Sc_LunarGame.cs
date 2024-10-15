using Scenes;
using LunarLander.Physics;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using LunarLander.Input;
using Microsoft.Xna.Framework.Input;
using LunarLander.Assets.GameObjects;
using LunarLander.Assets.Menu.GameUI;
using LunarLander.Entities.GameObjects;
using LunarLander.Assets.ParticleSystems;
using System;
using LunarLander.Assets.ParticleSystems.Explosion;
using LunarLander.Assets.GameData;
using LunarLander.ECS.Entities.UI_Items;

namespace LunarLander.Scenes.Scenes
{
    // Scene containing main gameplay content.
    public class Sc_LunarGame : SceneBase
    {
        // Gravity data.
        public Gravity gravity;

        // Terrain.
        public Terrain terrain;

        // Lander.
        public Lander lander;

        // Terrain collision handling.
        public TerrainCollisionHandling terrainCollisionHandling;

        // Game UI display.
        public GameUI gameUI;

        // Level message.
        public LevelMessage levelMessage;

        // Command packets associated with scene.
        CommandPacket thrust_CPkt;
        CommandPacket rotateLeft_CPkt;
        CommandPacket rotateRight_CPkt;

        // Position to place lander on when level begins.
        private Vector2 initialPosition = new Vector2(640.0f, 120.0f);

        // Background element.
        private Rectangle backgroundRectangle;

        // Timer for displaying the level message.
        private TimeSpan messageTimer;

        // Timer for countdown display.
        private TimeSpan countdownTimerInterval;

        // Current timer for various timed events.
        private TimeSpan currentTimer;

        // Start value for countdown.
        private int countDownValueStart = 3;

        // Current countdown value.
        private int countDownValue;

        // Color for good status message.
        private Color goodColor = new Color(115, 255, 33);

        // Background color for frame for level messages.
        private Color transparentBackgroundColor = new Color(36, 36, 36, 215);

        // Origin offset to transparent background frame.
        private Vector2 transparentBackgroundOriginOffset;

        // The size of transparent background frame.
        private Vector2 transparentBackgroundSize = new Vector2(700, 100);

        // Position of the center of the screen.
        private Vector2 centerScreenPosition;

        // Initializes state of LunarGame scene.
        public override void Initialize()
        {
            gravity = new Gravity();
            terrain = new Terrain(Managers.graphicsManager.bounds);
            backgroundRectangle = new Rectangle(0, 0, Managers.graphicsManager.WINDOW_WIDTH, Managers.graphicsManager.WINDOW_HEIGHT);

            transparentBackgroundOriginOffset = new Vector2((float)Managers.graphicsManager.rectColorTexture.Width / 2, (float)Managers.graphicsManager.rectColorTexture.Height / 2);
            centerScreenPosition = new Vector2(Managers.graphicsManager.WINDOW_WIDTH / 2, Managers.graphicsManager.WINDOW_HEIGHT / 2);

            terrainCollisionHandling = new TerrainCollisionHandling();
            lander = new Lander(Managers.graphicsManager.landerTexture, gravity, initialPosition, Managers.graphicsManager.bounds);
            gameUI = new GameUI(lander);
            levelMessage = new LevelMessage();

            messageTimer = new TimeSpan(0, 0, 3);
            countdownTimerInterval = new TimeSpan(0, 0, 1);
            currentTimer = new TimeSpan(0, 0, 0);
        }

        // Registers CommandPackets into the Input Manager.
        public override void RegisterCommandPackets()
        {
            thrust_CPkt = new CommandPacket(CommandMode.PRESS_REGULAR, Thrust, Unthrust, "Thrust");
            rotateLeft_CPkt = new CommandPacket(CommandMode.PRESS_REGULAR, RotateLanderLeft, null, "Rotate Left");
            rotateRight_CPkt = new CommandPacket(CommandMode.PRESS_REGULAR, RotateLanderRight, null, "Rotate Right");

            Managers.inputManager.inputMap.RegisterCommandPacket(thrust_CPkt.commandName, thrust_CPkt);
            Managers.inputManager.inputMap.RegisterCommandPacket(rotateLeft_CPkt.commandName, rotateLeft_CPkt);
            Managers.inputManager.inputMap.RegisterCommandPacket(rotateRight_CPkt.commandName, rotateRight_CPkt);
        }

        // Registers default input for Input Manager if no InputMap was loaded via persistent storage.
        public override void RegisterDefaultKeys()
        {
            Managers.inputManager.inputMap.RegisterKey(Keys.Up, InputContext.GAME, thrust_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.Left, InputContext.GAME, rotateLeft_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.Right, InputContext.GAME, rotateRight_CPkt.commandName);
        }

        // Rotates the lander left.
        public void RotateLanderLeft(GameTime gameTime)
        {
            lander.UpdateAngle(false, gameTime);
        }

        // Rotates the lander right.
        public void RotateLanderRight(GameTime gameTime)
        {
            lander.UpdateAngle(true, gameTime);
        }

        // Applies thrust to the lander.
        public void Thrust(GameTime gameTime)
        {
            lander.ApplyThrust(gameTime);
        }

        // Deactivates thrust.
        public void Unthrust(GameTime gameTime)
        {
            lander.Unthrust();
        }

        // State for when the main game scene is started.
        public void InitialMessageState(GameTime gameTime)
        {
            // Display countdown.
            currentTimer += gameTime.ElapsedGameTime;
            if (currentTimer >= countdownTimerInterval)
            {
                Managers.audioManager.src_messageBeep.Play();
                countDownValue += -1;
                levelMessage.UpdateMessageText(countDownValue.ToString());
                currentTimer = TimeSpan.Zero;
            }

            // Transition state to main gameplay state.
            if (countDownValue <= 0)
            {
                TransitionState(MainGamePlay_Update, MainGamePlay_Render, InputContext.GAME, InputMode.COMMAND_MODE);
            }
        }

        // State for when the game is transitioning levels.
        public void LevelTransitionState(GameTime gameTime)
        {
            // Generate new level and data if safe zone count is bigger than zero.
            if (Managers.gameDataManager.sessionData.safeZoneCount > 0)
            {
                // Display the timer.
                currentTimer += gameTime.ElapsedGameTime;
                if (currentTimer >= countdownTimerInterval)
                {
                    Managers.audioManager.src_messageBeep.Play();
                    countDownValue += -1;
                    levelMessage.UpdateMessageText(countDownValue.ToString());
                    currentTimer = TimeSpan.Zero;
                }

                // Reset game data and generate a new level.
                if (countDownValue <= 0)
                {
                    // Clear terrain data.
                    terrain.ClearData();

                    // Clear remaining particles.
                    ParticleSystem.ClearParticles();

                    // Make safe zone areas now small.
                    terrain.currentSafeZoneMin = terrain.smallSafeZoneLength_MIN;
                    terrain.currentSafeZoneMax = terrain.smallSafeZoneLength_MAX;

                    // Generate the terrain.
                    terrain.GenerateTerrain(Managers.gameDataManager.sessionData.safeZoneCount);
                    terrainCollisionHandling.InjectTerrainData(terrain.lineList);

                    // Clear level message.
                    levelMessage.UpdateMessageText("");

                    // Reset the lander
                    lander.ResetLander(initialPosition);

                    // Transition to main gameplay state.
                    TransitionState(MainGamePlay_Update, MainGamePlay_Render, InputContext.GAME, InputMode.COMMAND_MODE);
                }
            }
            else
            {
                // Clear terrain data.
                terrain.ClearData();

                // Clear remaining particles.
                ParticleSystem.ClearParticles();

                // Reset the lander
                lander.ResetLander(initialPosition);

                // Save score data.
                Managers.gameDataManager.highScoresList.latestScore = Managers.gameDataManager.sessionData.score;
                ScoreEntry newEntry = new ScoreEntry(Managers.gameDataManager.highScoresList.latestScore, "");

                // Get reference to menu scene.
                Sc_Menu mainMenu = (Sc_Menu)Managers.sceneManager.scenes[SceneLabel.MAIN_MENU];

                // Update score data if high score was achieved.
                if (Managers.gameDataManager.highScoresList.AddScore(newEntry))
                {
                    Managers.gameDataManager.scoreData_IO.SaveScoresList();
                    Managers.inputManager.workingString = "";
                    Managers.inputManager.SetCharacterLimit(15);
                    mainMenu.scoresMenu.ClearSelections();
                    mainMenu.scoresMenu.currentScoreEnterIndex = Managers.gameDataManager.highScoresList.latestHighScoreInputIndex + mainMenu.scoresMenu.startingScoreItemIndex;
                    UI_TextPair scoreEntry = (UI_TextPair)(mainMenu.scoresMenu.menuDecor[mainMenu.scoresMenu.currentScoreEnterIndex]);
                    mainMenu.scoresMenu.currentScoreTextEnter = scoreEntry.leftText_UI;
                    mainMenu.scoresMenu.drawInstructions = true;

                    // Change input mode to type mode for player name input.
                    mainMenu.TransitionState(mainMenu.HighScoreEnter_Update, mainMenu.HighScoreEnter_Render, InputContext.MENU, InputMode.TYPE_MODE);
                }
                else
                {
                    // Change input mode to command mode.
                    mainMenu.TransitionState(mainMenu.MainMenu_Update, mainMenu.MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
                }

                // Refresh latest score etnry.
                mainMenu.scoresMenu.RefreshList();

                // Transition to score menu.
                Managers.sceneManager.TransitionScene(SceneLabel.MAIN_MENU);
                mainMenu.currentMenu = mainMenu.scoresMenu;

                // Clear gameUI display for next round.
                gameUI.UpdateDisplayData();
            }
        }

        // Render the start state.
        public void StartState_Render()
        {
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            Managers.graphicsManager.spriteBatch.Begin();
            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.backgroundTexture,
              backgroundRectangle,
              Color.White
            );
            Managers.graphicsManager.spriteBatch.End();

            terrain.DrawFill();

            Managers.graphicsManager.spriteBatch.Begin();
            terrain.DrawOutline();
            Managers.graphicsManager.spriteBatch.End();

            ParticleSystem.DrawEmitterParticles();

            Managers.graphicsManager.spriteBatch.Begin();
            lander.Draw();
            gameUI.Draw();
            Managers.graphicsManager.spriteBatch.Draw(Managers.graphicsManager.rectColorTexture, centerScreenPosition, null, transparentBackgroundColor, 0, transparentBackgroundOriginOffset, transparentBackgroundSize, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            levelMessage.Draw();
            Managers.graphicsManager.spriteBatch.End();
        }

        // Main gameplay state.
        public void MainGamePlay_Update(GameTime gameTime)
        {
            // Process currently pressed commands.
            Managers.inputManager.ProcessCommands(gameTime);

            // Update lander.
            lander.ApplyGravity(gameTime);
            lander.UpdatePosition();

            // Update GameUI display.
            gameUI.UpdateDisplayData();

            // Update any active emitters.
            ParticleSystem.UpdateEmitters(gameTime);

            // Handle collision.
            CollisionState landerCollided = terrainCollisionHandling.TestTerrainCollision(lander.collider);
            if (landerCollided != CollisionState.NO_COLLIDE)
            {
                // Grab lander data needed for landing.
                float landerSpeed = lander.CurrentSpeed();
                float degreeAngle = lander.DegreeRotation();

                // Check if lander is ready to land.
                if ((landerSpeed >= 0 && landerSpeed < 2) && ((degreeAngle >= 355 && degreeAngle <= 360) || (degreeAngle >= 0 && degreeAngle <= 5)) && landerCollided == CollisionState.SAFE_COLLIDE)
                {
                    // Successful landing.

                    // Play and stop corresponding audio.
                    Managers.audioManager.rocketRumble.Stop();
                    Managers.audioManager.alert.Stop();
                    Managers.audioManager.src_success.Play();

                    // Update UI displays.
                    levelMessage.UpdateColor(goodColor);
                    levelMessage.UpdateMessageText("Success! Fuel added to score");

                    // Update score info.
                    Managers.gameDataManager.sessionData.score += (int)lander.fuel;

                    // Reset timer.
                    currentTimer = TimeSpan.Zero;

                    // Transition states to victory state.
                    TransitionState(Victory_Update, Victory_Render, InputContext.GAME, InputMode.DISABLED_MODE);
                }
                else
                {
                    // Landing failed.

                    // Play and stop corresponding audio.
                    Managers.audioManager.gameMusic.Stop();
                    Managers.audioManager.rocketRumble.Stop();
                    Managers.audioManager.alert.Stop();
                    Managers.audioManager.src_explosion.Play();

                    // Explode the ship.
                    lander.ExplodeEffect();

                    // Display level message.
                    levelMessage.UpdateColor(Color.Red);
                    levelMessage.UpdateMessageText("Game Over");

                    // Reset timer.
                    currentTimer = TimeSpan.Zero;

                    // Transition to game over state.
                    TransitionState(GameOver_Update, GameOver_Render, InputContext.GAME, InputMode.DISABLED_MODE);
                }
            }
        }

        // Render main gameplay state.
        public void MainGamePlay_Render()
        {
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            Managers.graphicsManager.spriteBatch.Begin();
            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.backgroundTexture,
              backgroundRectangle,
              Color.White
            );

            Managers.graphicsManager.spriteBatch.End();

            terrain.DrawFill();

            Managers.graphicsManager.spriteBatch.Begin();
            terrain.DrawOutline();
            Managers.graphicsManager.spriteBatch.End();

            ParticleSystem.DrawEmitterParticles();

            Managers.graphicsManager.spriteBatch.Begin();
            lander.Draw();
            gameUI.Draw();
            Managers.graphicsManager.spriteBatch.End();
        }

        // Victory state.
        public void Victory_Update(GameTime gameTime)
        {
            // Display victory message.
            currentTimer += gameTime.ElapsedGameTime;
            if (currentTimer >= messageTimer)
            {
                // Reset timer.
                currentTimer = TimeSpan.Zero;

                // Setup countdown.
                countDownValue = countDownValueStart;
                levelMessage.UpdateColor(Color.White);
                Managers.audioManager.src_messageBeep.Play();
                levelMessage.UpdateMessageText(countDownValue.ToString());

                // Update safezone count for next level.
                Managers.gameDataManager.sessionData.safeZoneCount += -1;

                // Transition to level transition state.
                TransitionState(LevelTransitionState, Victory_Render, InputContext.GAME, InputMode.DISABLED_MODE);
            }

            // Update any active particles.
            ParticleSystem.UpdateEmitters(gameTime);
        }

        // Render the victory state.
        public void Victory_Render()
        {
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            Managers.graphicsManager.spriteBatch.Begin();
            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.backgroundTexture,
              backgroundRectangle,
              Color.White
            );
            Managers.graphicsManager.spriteBatch.End();

            terrain.DrawFill();

            Managers.graphicsManager.spriteBatch.Begin();
            terrain.DrawOutline();
            Managers.graphicsManager.spriteBatch.End();

            ParticleSystem.DrawEmitterParticles();

            Managers.graphicsManager.spriteBatch.Begin();
            lander.Draw();
            gameUI.Draw();
            Managers.graphicsManager.spriteBatch.Draw(Managers.graphicsManager.rectColorTexture, centerScreenPosition, null, transparentBackgroundColor, 0, transparentBackgroundOriginOffset, transparentBackgroundSize, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            levelMessage.Draw();

            Managers.graphicsManager.spriteBatch.End();
        }

        // Game over state.
        public void GameOver_Update(GameTime gameTime)
        {
            // Display game over message.
            currentTimer += gameTime.ElapsedGameTime;
            if (currentTimer >= messageTimer)
            {
                // Clear terrain data.
                terrain.ClearData();

                // Clear remaining particles.
                ParticleSystem.ClearParticles();

                // Reset the lander
                lander.ResetLander(initialPosition);

                // Update score data.
                Managers.gameDataManager.highScoresList.latestScore = Managers.gameDataManager.sessionData.score;
                ScoreEntry newEntry = new ScoreEntry(Managers.gameDataManager.highScoresList.latestScore, "");

                // Get reference to menu scene.
                Sc_Menu mainMenu = (Sc_Menu)Managers.sceneManager.scenes[SceneLabel.MAIN_MENU];

                // If high score achieved, do more updates to score data.
                if (Managers.gameDataManager.highScoresList.AddScore(newEntry))
                {
                    Managers.gameDataManager.scoreData_IO.SaveScoresList();
                    Managers.inputManager.workingString = "";
                    Managers.inputManager.SetCharacterLimit(15);
                    mainMenu.scoresMenu.ClearSelections();
                    mainMenu.scoresMenu.currentScoreEnterIndex = Managers.gameDataManager.highScoresList.latestHighScoreInputIndex + mainMenu.scoresMenu.startingScoreItemIndex;
                    UI_TextPair scoreEntry = (UI_TextPair)(mainMenu.scoresMenu.menuDecor[mainMenu.scoresMenu.currentScoreEnterIndex]);
                    mainMenu.scoresMenu.currentScoreTextEnter = scoreEntry.leftText_UI;
                    mainMenu.scoresMenu.drawInstructions = true;

                    // Set input mode to type mode for player name input.
                    mainMenu.TransitionState(mainMenu.HighScoreEnter_Update, mainMenu.HighScoreEnter_Render, InputContext.MENU, InputMode.TYPE_MODE);
                }
                else
                {
                    // Set input mode to command mode.
                    mainMenu.TransitionState(mainMenu.MainMenu_Update, mainMenu.MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
                }

                // Refresh latest score entry.
                mainMenu.scoresMenu.RefreshList();

                // Transition to scores menu.
                Managers.sceneManager.TransitionScene(SceneLabel.MAIN_MENU);
                mainMenu.currentMenu = mainMenu.scoresMenu;

                // Reset GameUI for next game session.
                gameUI.UpdateDisplayData();
            }

            // Update any active particles.
            ParticleSystem.UpdateEmitters(gameTime);
        }

        // Render the game over state.
        public void GameOver_Render()
        {
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            Managers.graphicsManager.spriteBatch.Begin();
            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.backgroundTexture,
              backgroundRectangle,
              Color.White
            );
            Managers.graphicsManager.spriteBatch.End();

            terrain.DrawFill();

            Managers.graphicsManager.spriteBatch.Begin();
            terrain.DrawOutline();
            Managers.graphicsManager.spriteBatch.End();

            ParticleSystem.DrawEmitterParticles();

            Managers.graphicsManager.spriteBatch.Begin();
            gameUI.Draw();
            Managers.graphicsManager.spriteBatch.Draw(Managers.graphicsManager.rectColorTexture, centerScreenPosition, null, transparentBackgroundColor, 0, transparentBackgroundOriginOffset, transparentBackgroundSize, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            levelMessage.Draw();

            Managers.graphicsManager.spriteBatch.End();
        }

        // Called when the scene is entered into.
        public override void EnterState()
        {
            // Update background music.
            Managers.audioManager.gameMusic.Play();

            // Reset session data.
            Managers.gameDataManager.sessionData.ResetData();

            // Reset the timer.
            currentTimer = TimeSpan.Zero;

            // Make safe zone areas big for first level.
            terrain.currentSafeZoneMin = terrain.bigSafeZoneLength_MIN;
            terrain.currentSafeZoneMax = terrain.bigSafeZoneLength_MAX;

            // Generate the terrain.
            terrain.GenerateTerrain(Managers.gameDataManager.sessionData.safeZoneCount);

            // Setup collision.
            terrainCollisionHandling.InjectTerrainData(terrain.lineList);

            // Display level message.
            Managers.audioManager.src_messageBeep.Play();
            levelMessage.UpdateColor(Color.White);
            countDownValue = countDownValueStart;
            levelMessage.UpdateMessageText(countDownValue.ToString());

            // Transition input mode to disabled mode temporarily while messages are being displayed.
            TransitionState(InitialMessageState, StartState_Render, InputContext.GAME, InputMode.DISABLED_MODE);
        }

        // Called when the scene is exited.
        public override void ExitState()
        {
            // Disable any remaining sounds.
            Managers.audioManager.rocketRumble.Stop();
            Managers.audioManager.alert.Stop();
            Managers.audioManager.gameMusic.Stop();
        }
    }
}