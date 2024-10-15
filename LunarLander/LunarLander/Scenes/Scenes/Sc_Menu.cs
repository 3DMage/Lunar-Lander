using Scenes;
using LunarLander.Utilities;
using LunarLander.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using LunarLander.ECS.Entities.Menu.Menus;
using LunarLander.Entities.Menu;

namespace LunarLander.Scenes.Scenes
{
    // Scene containing main menu content.
    public class Sc_Menu : SceneBase
    {
        // Current menu being drawn and operated on.
        public Menu currentMenu;

        // Menus
        public ScoresMenu scoresMenu { get; private set; }
        public MainMenu mainMenu { get; private set; }
        public SettingsMenu settingsMenu { get; private set; }
        public CreditsMenu creditsMenu { get; private set; }

        // Background element.
        private Rectangle backgroundRectangle;

        // Current input config being used.
        public InputConfigEntry currentInputConfigEntry { get; set; }

        // Command packets associated with scene.
        CommandPacket moveSelectionUp_CPkt;
        CommandPacket moveSelectionDown_CPkt;
        CommandPacket confirmSelection_CPkt;
        CommandPacket returnToMainMenu_CPkt;

        // Initializes state of Menu scene.
        public override void Initialize()
        {
            mainMenu = new MainMenu(this);
            scoresMenu = new ScoresMenu(this);
            settingsMenu = new SettingsMenu(this);
            creditsMenu = new CreditsMenu(this);

            mainMenu.ConstructMenu();
            scoresMenu.ConstructMenu();
            settingsMenu.ConstructMenu();
            creditsMenu.ConstructMenu();

            currentMenu = mainMenu;

            backgroundRectangle = new Rectangle(0, 0, Managers.graphicsManager.WINDOW_WIDTH, Managers.graphicsManager.WINDOW_HEIGHT);

            TransitionState(MainMenu_Update, MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
        }

        // Registers CommandPackets into the Input Manager.
        public override void RegisterCommandPackets()
        {
            moveSelectionUp_CPkt = new CommandPacket(CommandMode.PRESS_ONLY, MoveSelectionUp, null, "Move Selection Up");
            moveSelectionDown_CPkt = new CommandPacket(CommandMode.PRESS_ONLY, MoveSelectionDown, null, "Move Selection Down");
            confirmSelection_CPkt = new CommandPacket(CommandMode.PRESS_ONLY, ConfirmSelection, null, "Confirm Selection");
            returnToMainMenu_CPkt = new CommandPacket(CommandMode.PRESS_ONLY, ReturnToMainMenu, null, "Return to Main Menu");

            Managers.inputManager.inputMap.RegisterCommandPacket(moveSelectionUp_CPkt.commandName, moveSelectionUp_CPkt);
            Managers.inputManager.inputMap.RegisterCommandPacket(moveSelectionDown_CPkt.commandName, moveSelectionDown_CPkt);
            Managers.inputManager.inputMap.RegisterCommandPacket(confirmSelection_CPkt.commandName, confirmSelection_CPkt);
            Managers.inputManager.inputMap.RegisterCommandPacket(returnToMainMenu_CPkt.commandName, returnToMainMenu_CPkt);
        }

        // Registers default input for Input Manager if no InputMap was loaded via persistent storage.
        public override void RegisterDefaultKeys()
        {
            Managers.inputManager.inputMap.RegisterKey(Keys.Up, InputContext.MENU, moveSelectionUp_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.W, InputContext.MENU, moveSelectionUp_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.Down, InputContext.MENU, moveSelectionDown_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.S, InputContext.MENU, moveSelectionDown_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.Enter, InputContext.MENU, confirmSelection_CPkt.commandName);
            Managers.inputManager.inputMap.RegisterKey(Keys.Escape, InputContext.MENU, returnToMainMenu_CPkt.commandName);
        }

        // Moves currently selected menu item upwards.
        public void MoveSelectionUp(GameTime gameTime)
        {
            Managers.audioManager.src_moveSelectBeep.Play();
            currentMenu.menuOptions[currentMenu.selectionIndex].UnmarkSelected();
            currentMenu.selectionIndex = MathKit.mod(currentMenu.selectionIndex - 1, currentMenu.menuOptions.Count);
            currentMenu.menuOptions[currentMenu.selectionIndex].MarkSelected();
        }

        // Moves currently selected menu item downwards.
        public void MoveSelectionDown(GameTime gameTime)
        {
            Managers.audioManager.src_moveSelectBeep.Play();
            currentMenu.menuOptions[currentMenu.selectionIndex].UnmarkSelected();
            currentMenu.selectionIndex = MathKit.mod(currentMenu.selectionIndex + 1, currentMenu.menuOptions.Count);
            currentMenu.menuOptions[currentMenu.selectionIndex].MarkSelected();
        }

        // Confirms selection and activates button action.
        public void ConfirmSelection(GameTime gameTime)
        {
            Managers.audioManager.src_selectBeep.Play();
            currentMenu.menuOptions[currentMenu.selectionIndex].ButtonAction();
        }

        // Goes to main menu from current menu.
        public void ReturnToMainMenu(GameTime gameTime)
        {
            Managers.audioManager.src_selectBeep.Play();
            currentMenu = mainMenu;
        }

        // Called when the scene is entered into.
        public override void EnterState()
        {
            Managers.audioManager.menuMusic.Play();
        }

        // Called when the scene is exited.
        public override void ExitState()
        {
            Managers.audioManager.menuMusic.Stop();
        }

        // Normal menu state.
        public void MainMenu_Update(GameTime gameTime)
        {
            Managers.inputManager.ProcessCommands(gameTime);
        }

        // Render the menu state.
        public void MainMenu_Render()
        {
            // Clear frame.
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            // Begin drawing.
            Managers.graphicsManager.spriteBatch.Begin();

            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.menuBackgroundTexture,
              backgroundRectangle,
              Color.White
            );

            currentMenu.Draw();

            // If the item is selected, draw that, else draw non-selected.
            Managers.graphicsManager.spriteBatch.End();
        }

        // High score state.
        public void HighScoreEnter_Update(GameTime gameTime)
        {
            Managers.inputManager.GetTypedCharacters();
            Managers.gameDataManager.highScoresList.scores[Managers.gameDataManager.highScoresList.latestHighScoreInputIndex].name = Managers.inputManager.workingString;
            scoresMenu.RefreshList();
            scoresMenu.currentScoreTextEnter.UpdateBlinker(gameTime);

            if (Managers.inputManager.doneTyping)
            {
                Managers.audioManager.src_selectBeep.Play();
                Managers.gameDataManager.scoreData_IO.SaveScoresList();
                scoresMenu.selectionIndex = 0;
                scoresMenu.menuOptions[scoresMenu.selectionIndex].MarkSelected();
                scoresMenu.drawInstructions = false;
                scoresMenu.currentScoreTextEnter.ClearBlinker();
                Managers.inputManager.doneTyping = false;
                TransitionState(MainMenu_Update, MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
            }
        }

        // Render the high score state.
        public void HighScoreEnter_Render()
        {
            // Clear frame.
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            // Begin drawing.
            Managers.graphicsManager.spriteBatch.Begin();

            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.menuBackgroundTexture,
              backgroundRectangle,
              Color.White
            );

            currentMenu.Draw();

            // If the item is selected, draw that, else draw non-selected.
            Managers.graphicsManager.spriteBatch.End();
        }

        // Input change state.
        public void InputChange_Update(GameTime gameTime)
        {
            ConfigState remapped = Managers.inputManager.ConfigKey(currentInputConfigEntry.inputContext, currentInputConfigEntry.key);
            if (remapped == ConfigState.VALID)
            {
                Managers.audioManager.src_selectBeep.Play();
                Keys newKey = Managers.inputManager.latestPressedKey;
                //? REFINEMENT NOTE: The currentInputConfig entry should be tracked by settings mneu, not Sc_Menu!
                currentInputConfigEntry.UpdateKey(newKey);
                settingsMenu.ClearMessages();
                Managers.inputManager.inputMap_IO.SaveInputMap();
                TransitionState(MainMenu_Update, MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
            }
            else if (remapped == ConfigState.ITSELF || remapped == ConfigState.CANCELED)
            {
                Managers.audioManager.src_moveSelectBeep.Play();
                currentInputConfigEntry.UpdateKey(currentInputConfigEntry.key);
                settingsMenu.ClearMessages();
                TransitionState(MainMenu_Update, MainMenu_Render, InputContext.MENU, InputMode.COMMAND_MODE);
            }
            else if (remapped == ConfigState.ALREADY_MAPPED)
            {
                Managers.audioManager.src_moveSelectBeep.Play();
                settingsMenu.UpdateStatusMessage("Key already mapped.  Use a different key.");
            }
        }

        // Render the input change state.
        public void InputChange_Render()
        {
            // Clear frame.
            Managers.graphicsManager.graphicsDevice.Clear(Color.Black);

            // Begin drawing.
            Managers.graphicsManager.spriteBatch.Begin();

            Managers.graphicsManager.spriteBatch.Draw
            (
              Managers.graphicsManager.menuBackgroundTexture,
              backgroundRectangle,
              Color.White
            );

            currentMenu.Draw();

            // If the item is selected, draw that, else draw non-selected.
            Managers.graphicsManager.spriteBatch.End();
        }
    }
}
