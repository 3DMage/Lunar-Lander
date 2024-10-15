using LunarLander.ECS.Entities.Menu;
using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;

namespace LunarLander.Entities.Menu
{
    // Transition button menu item.  Transitions between menus.
    public class TransitionButton : MenuItem
    {
        // Reference to menu scene.
        Sc_Menu menuScene;

        // Target menu to go to.
        LunarLander.ECS.Entities.Menu.Menus.Menu target;

        // Constructor.
        public TransitionButton(UI_Item uiItem, Sc_Menu menuScene, LunarLander.ECS.Entities.Menu.Menus.Menu target) : base(uiItem)
        {
            this.menuScene = menuScene;
            this.target = target;
        }

        // Action to execute when button is initiated.  In this case, it makes menu transition to the target menu.
        public override void ButtonAction()
        {
            menuScene.currentMenu = target;
        }
    }
}
