using LunarLander.ECS.Entities.UI_Items;
using LunarLander.Entities.Menu;
using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace LunarLander.ECS.Entities.Menu.Menus
{
    // Menu displaying credits to the game.
    public class CreditsMenu : Menu
    {
        // Constructor.
        public CreditsMenu(Sc_Menu menuScene) : base(menuScene) { }

        // Center X coordinate.
        private int centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);

        // Constructs the menu.
        public override void ConstructMenu()
        {
            // Menu coordinates.
            int leftX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.05f);
            centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);
            int bottomY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.95f);
            int header_positionY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.05f);

            // Header element.
            UI_Text headerLabel_UI = new UI_Text(centerX, header_positionY, UI_Origin.CENTER, Managers.graphicsManager.spaceFontHeader, Color.White, "Credits");

            // Y positions of all credit entries.
            int Y_position1 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.20);
            int Y_position2 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.24);
            int Y_position3_1 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.28);
            int Y_position3_2 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.32);
            int Y_position4 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.36);
            int Y_position5 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.44);
            int Y_position6 = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.48);

            // Credit entries.
            UI_TextPair creditsEntry1 = new UI_TextPair(centerX, Y_position1, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 40, "Programming:", "Benjamin Ricks");
            UI_TextPair creditsEntry2 = new UI_TextPair(centerX, Y_position2, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 40, "Graphics:", "Benjamin Ricks");
            UI_TextPair creditsEntry3_1 = new UI_TextPair(centerX, Y_position3_1, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 40, "Menu Music:", "Loop-Menu - Akikazer");
            UI_TextPair creditsEntry3_2 = new UI_TextPair(centerX, Y_position3_2, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 40, "Game Music:", "Nomogus41 - Centurion_of_war");
            UI_TextPair creditsEntry4 = new UI_TextPair(centerX, Y_position4, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 40, "Confirm Beep Effect:", "Confirm Beeps - Dylan Kelk");
            UI_Text creditsEntry5 = new UI_Text(centerX, Y_position5, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "All other assets used are in the public domain found");
            UI_Text creditsEntry6 = new UI_Text(centerX, Y_position6, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "in OpenGameArt.org and FreeSound.org.");

            // Add non-interactive elements to menuDecor.
            menuDecor.Add(headerLabel_UI);
            menuDecor.Add(creditsEntry1);
            menuDecor.Add(creditsEntry2);
            menuDecor.Add(creditsEntry3_1);
            menuDecor.Add(creditsEntry3_2);
            menuDecor.Add(creditsEntry4);
            menuDecor.Add(creditsEntry5);
            menuDecor.Add(creditsEntry6);

            // Back button element.
            UI_Text backButton_UI = new UI_Text(leftX, bottomY, UI_Origin.CENTER_LEFT, Managers.graphicsManager.spaceFont, Color.White, "Back");

            // Add interactive elements to menu options.
            menuOptions.Add(new TransitionButton(backButton_UI, menuScene, menuScene.mainMenu));

            // Mark currently selected element.
            menuOptions[selectionIndex].MarkSelected();
        }
    }
}