using Microsoft.Xna.Framework;

namespace LunarLander.Entities.UI_Items
{
    // Base class for UI_Item objects.
    public abstract class UI_Item
    {
        // Position of the UI element.
        public Vector2 position { get; protected set; }

        // Offset to position object relative to the custom origin.
        public Vector2 originOffset { get; protected set; }

        // Label indicating location of the origin.
        public UI_Origin originType { get; private set; }

        // Color of the UI_Item.
        public Color color { get; set; }

        // Constructor.
        public UI_Item(int screenPos_X, int screenPos_Y, UI_Origin origin, Color color)
        {
            this.color = color;
            position = new Vector2(screenPos_X, screenPos_Y);
            originOffset = new Vector2(0, 0);
            this.originType = origin;
        }

        // Abstract method for how to draw the UI element.
        public abstract void Draw_UI();
    }
}
