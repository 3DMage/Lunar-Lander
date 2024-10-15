using LunarLander.Entities.UI_Items;
using Microsoft.Xna.Framework;

namespace LunarLander.ECS.Entities.Menu
{
    // Base class for MenuItem objects.
    public abstract class MenuItem
    {
        // UI element associated with the MenuItem.
        public UI_Item uiItem;

        // Abstract method for what action to activate when button is initiated.
        public abstract void ButtonAction();

        // Default color of unselected state of UI element.
        public Color defaultColor = Color.White;

        // Selection color of selected state of UI element.
        public Color selectionColor = new Color(255, 205, 41);

        // Constructor.
        public MenuItem(UI_Item uiItem)
        {
            this.uiItem = uiItem;
        }

        // Marks an element with selection color.
        public virtual void MarkSelected()
        {
            uiItem.color = selectionColor;
        }

        // Marks an element with default color.
        public virtual void UnmarkSelected()
        {
            uiItem.color = defaultColor;
        }
    }
}
