using LunarLander.Entities.UI_Items;
using LunarLander.Utilities;
using Scenes;

namespace LunarLander.ECS.Entities.Menu
{
    // Play button menu item.
    public class PlayButton : MenuItem
    {
        // Constructor.
        public PlayButton(UI_Item uiItem) : base(uiItem) { }

        // Action to execute when button is initiated.  In this case, it makes the game go into the main lunar lander game.
        public override void ButtonAction()
        {
            Managers.sceneManager.TransitionScene(SceneLabel.GAME);
        }
    }
}
