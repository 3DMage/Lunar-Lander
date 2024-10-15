using LunarLander.ECS.Entities.UI_Items;
using LunarLander.Entities.Menu;
using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using LunarLander.Input;

namespace LunarLander.ECS.Entities.Menu.Menus
{
    // Menu to config user input controls.
    public class SettingsMenu : Menu
    {
        // UI element displaying messages based on condition of input configuration.
        UI_Text statusMessage_UI;

        // UI element displaying instructions for input configuration.
        UI_Text instructionText_UI;

        // Initial message for instructionText_UI.
        string startingInstructions = "Press Enter to remap key.";

        // Constructor.
        public SettingsMenu(Sc_Menu menuScene) : base(menuScene) { }

        // Constructs the menu.
        public override void ConstructMenu()
        {
            // Menu coordinates.
            int leftX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.05f);
            int centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);
            int bottomY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.95f);
            int positionY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.05f);
            int positionY_2 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.75f);
            int positionY_3 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.79f);

            // Header element.
            UI_Text headerLabel_UI = new UI_Text(centerX, positionY, UI_Origin.CENTER, Managers.graphicsManager.spaceFontHeader, Color.White, "Settings");

            // Status display element.
            statusMessage_UI = new UI_Text(centerX, positionY_2, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.Red, "");

            // Instructions element.
            instructionText_UI = new UI_Text(centerX, positionY_3, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, startingInstructions);

            // Add non-interactive elements to menuDecor.
            menuDecor.Add(headerLabel_UI);
            menuDecor.Add(statusMessage_UI);
            menuDecor.Add(instructionText_UI);

            // Compute Y coordinates for each input config entry.
            float currentOffset = 0.3f;
            int currentY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * currentOffset);

            foreach (KeyValuePair<(InputContext, Keys), string> kvp in Managers.inputManager.inputMap.keyMap)
            {
                if (kvp.Key.Item1 == InputContext.GAME && kvp.Key.Item2 != Keys.Escape)
                {
                    UI_TextPair entry = new UI_TextPair(centerX, currentY, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 400, kvp.Value, kvp.Key.Item2.ToString());
                    menuOptions.Add(new InputConfigEntry(entry, kvp.Key.Item2, kvp.Key.Item1, menuScene));
                    currentOffset += 0.04f;
                    currentY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * currentOffset);
                }
            }

            // Back button element.
            UI_Text backButton_UI = new UI_Text(leftX, bottomY, UI_Origin.CENTER_LEFT, Managers.graphicsManager.spaceFont, Color.White, "Back");

            // Add interactive elements to menu options.
            menuOptions.Add(new TransitionButton(backButton_UI, menuScene, menuScene.mainMenu));

            // Mark currently selected element.
            menuOptions[selectionIndex].MarkSelected();
        }

        // Updates the status message.
        public void UpdateStatusMessage(string text)
        {
            statusMessage_UI.SetText(text);
        }

        // Updates the instructions message.
        public void UpdateInstructionMessage(string text)
        {
            instructionText_UI.SetText(text);
        }

        // Clears the message.
        public void ClearMessages()
        {
            statusMessage_UI.SetText("");
            instructionText_UI.SetText(startingInstructions);
        }
    }
}
