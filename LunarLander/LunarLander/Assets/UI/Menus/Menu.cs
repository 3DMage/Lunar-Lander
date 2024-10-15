using LunarLander.Entities.UI_Items;
using LunarLander.Scenes.Scenes;
using System.Collections.Generic;

namespace LunarLander.ECS.Entities.Menu.Menus
{
    // Base class for Menu objects.
    public abstract class Menu
    {
        // Reference to scene holding menus.
        protected Sc_Menu menuScene;

        // All non-interactive elements of the menu.
        public List<UI_Item> menuDecor { get; set; }

        // All interactable elements of the menu.
        public List<MenuItem> menuOptions { get; set; }

        // Index of currently selected menu option.
        public int selectionIndex { get; set; } = 0;

        // Toggle for activating or disabling menu selecting.
        protected bool selectionsEnabled = true;

        // Constructor.
        protected Menu(Sc_Menu menuScene)
        {
            this.menuScene = menuScene;
            menuDecor = new List<UI_Item>();
            menuOptions = new List<MenuItem>();
        }

        // Abstract method for constructing the menu.
        public abstract void ConstructMenu();

        // Draws the menu.
        public virtual void Draw()
        {
            // Draw decor
            for (int i = 0; i < menuDecor.Count; i++)
            {
                menuDecor[i].Draw_UI();
            }

            // Draw options
            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].uiItem.Draw_UI();
            }
        }

        // Clears all currently selected menu elements.
        public void ClearSelections()
        {
            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].UnmarkSelected();
            }
        }
    }
}
