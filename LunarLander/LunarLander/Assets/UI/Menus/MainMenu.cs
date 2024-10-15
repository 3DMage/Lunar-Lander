using LunarLander.Entities.Menu;
using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace LunarLander.ECS.Entities.Menu.Menus
{
    // Main menu for selecting other menus.
    public class MainMenu : Menu
    {
        // Constructor.
        public MainMenu(Sc_Menu menuScene) : base(menuScene) { }

        // Constructs the menu.
        public override void ConstructMenu()
        {
            // Coordinates for menu components.
            float scalingFactor = 0.6f;
            int centerX = (int)(Managers.graphicsManager.WINDOW_WIDTH * 0.5f);
            int positionY = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.05f);
            int sizeX = (int)(Managers.graphicsManager.titleLogo.Width * scalingFactor);
            int sizeY = (int)(Managers.graphicsManager.titleLogo.Height * scalingFactor);

            // Header element.
            UI_Picture titleLogo = new UI_Picture(centerX, positionY, sizeX, sizeY, UI_Origin.UPPER_CENTER, Managers.graphicsManager.titleLogo, Color.White);

            // Add non-interactive elements to menuDecor.
            menuDecor.Add(titleLogo);

            // Menu Items Y coordinates.
            int positionY_play = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.4f);
            int positionY_highScore = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.44f);
            int positionY_settings = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.48f);
            int positionY_credits = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.52f);
            int positionY_quit = (int)(Managers.graphicsManager.WINDOW_HEIGHT * 0.56f);

            // Menu buttons.
            UI_Text playButton_UI = new UI_Text(centerX, positionY_play, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "Play");
            UI_Text highScoreButton_UI = new UI_Text(centerX, positionY_highScore, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "High Score");
            UI_Text settingsButton_UI = new UI_Text(centerX, positionY_settings, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "Settings");
            UI_Text creditsButton_UI = new UI_Text(centerX, positionY_credits, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "Credits");
            UI_Text quitButton_UI = new UI_Text(centerX, positionY_quit, UI_Origin.CENTER, Managers.graphicsManager.spaceFont, Color.White, "Quit");

            // Add interactive elements to menu options.
            menuOptions.Add(new PlayButton(playButton_UI));
            menuOptions.Add(new TransitionButton(highScoreButton_UI, menuScene, menuScene.scoresMenu));
            menuOptions.Add(new TransitionButton(settingsButton_UI, menuScene, menuScene.settingsMenu));
            menuOptions.Add(new TransitionButton(creditsButton_UI, menuScene, menuScene.creditsMenu));
            menuOptions.Add(new QuitButton(quitButton_UI));

            // Mark currently selected element.
            menuOptions[selectionIndex].MarkSelected();
        }
    }
}
