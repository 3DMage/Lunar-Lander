using LunarLander.ECS.Entities.UI_Items;
using LunarLander.Entities.Menu;
using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace LunarLander.ECS.Entities.Menu.Menus
{
    // Menu for displaying high scores.
    public class ScoresMenu : Menu
    {
        // Center X coordinate of screen.
        private int centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);

        // The starting index within the menuDecor array for elements dealing with score entries.
        public int startingScoreItemIndex { get; private set; }

        // The index of menu item where the player is typing their name.
        public int currentScoreEnterIndex { get; set; } = -1;

        // The current UI_Text element to type player name on.
        public UI_Text currentScoreTextEnter { get; set; }

        // The instructions for telling the player to enter their name if they achieve a high score.
        public UI_Text playerNameInputInstructions { get; set; }

        // UI element to display latest score achieved from previous gameplay session.
        UI_Text latestScore_UI { get; set; }

        // Color to render playerNameInputInstructions text in.
        private Color instructionTextColor = new Color(115, 255, 33);

        // Toggle to draw the playerNameInputInstructions element.
        public bool drawInstructions { get; set; } = false;

        // Constructor.
        public ScoresMenu(Sc_Menu menuScene) : base(menuScene) { }

        // Constructs the menu.
        public override void ConstructMenu()
        {
            // Menu coordinates.
            int leftX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.05f);
            centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);
            int bottomY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.95f);
            int header_positionY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.05f);

            // Header element.
            UI_Text headerLabel_UI = new UI_Text(centerX, header_positionY, UI_Origin.CENTER, Managers.graphicsManager.spaceFontHeader, Color.White, "Scores");

            // Add header element to menuDecor.
            menuDecor.Add(headerLabel_UI);

            // This is index to menuDecor to start adding score entries from.
            startingScoreItemIndex = menuDecor.Count;

            // Compute Y positions of all score entries.
            float Y_Offset = 0.18f;
            float Y_increment = 0.04f;

            // Add all score entries from high score list data.
            for (int i = 0; i < Managers.gameDataManager.highScoresList.scores.Count; i++)
            {
                int Y_position = (int)(Managers.graphicsManager.WINDOW_HEIGHT * Y_Offset);
                UI_TextPair currentEntry = new UI_TextPair(centerX, Y_position, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, 100, "", "");
                menuDecor.Add(currentEntry);
                Y_Offset += Y_increment;
            }

            // Y coordinate to the instructions element.
            int instructionsY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.65f);

            // Player name input instructions UI element.
            playerNameInputInstructions = new UI_Text(centerX, instructionsY, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, instructionTextColor, "High score achieved!  Type your name and press enter.");

            // Y coordinate to latest score UI element.
            int latestScoreY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.75f);

            // Latest score display UI element.
            latestScore_UI = new UI_Text(centerX, latestScoreY, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "Latest Score: ");

            // Add the latest score UI element to menuDecor.
            menuDecor.Add(latestScore_UI);

            // Update text of all score entries.
            RefreshList();

            // Back button element.
            UI_Text backButton_UI = new UI_Text(leftX, bottomY, UI_Origin.CENTER_LEFT, Managers.graphicsManager.spaceFont, Color.White, "Back");

            // Add interactive elements to menu options.
            menuOptions.Add(new TransitionButton(backButton_UI, menuScene, menuScene.mainMenu));

            // Mark currently selected element.
            menuOptions[selectionIndex].MarkSelected();
        }

        // Draws the menu.
        public override void Draw()
        {
            base.Draw();

            // Only draw this if high score was achieved.
            if (drawInstructions)
            {
                playerNameInputInstructions.Draw_UI();
            }
        }

        // Updates text on all score entries.
        public void RefreshList()
        {
            latestScore_UI.SetText("Latest Score: " + Managers.gameDataManager.highScoresList.latestScore.ToString());

            int currentScoreItemIndex = startingScoreItemIndex;
            for (int i = 0; i < Managers.gameDataManager.highScoresList.scores.Count; i++)
            {
                UI_TextPair currentEntry = (UI_TextPair)menuDecor[currentScoreItemIndex];
                currentEntry.leftText_UI.SetText(Managers.gameDataManager.highScoresList.scores[i].name);
                currentEntry.rightText_UI.SetText(Managers.gameDataManager.highScoresList.scores[i].score.ToString());
                currentScoreItemIndex++;
            }
        }
    }
}
