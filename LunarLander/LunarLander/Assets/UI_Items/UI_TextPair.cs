using LunarLander.Entities.UI_Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LunarLander.ECS.Entities.UI_Items
{
    // UI element for displaying a pair of UI_Text elements.
    public class UI_TextPair : UI_Item
    {
        // The left text item.
        public UI_Text leftText_UI { get; set; }

        // The right text item.
        public UI_Text rightText_UI { get; set; }

        // The gap between the two text elements.
        public int gap { get; set; }

        // Constructor.
        public UI_TextPair(int screenPos_X, int screenPos_Y, UI_Origin origin, SpriteFont font, Color color, int gap, string leftText, string rightText) : base(screenPos_X, screenPos_Y, origin, color)
        {
            this.gap = gap;
            leftText_UI = new UI_Text(screenPos_X - gap / 2, screenPos_Y, UI_Origin.CENTER_RIGHT, font, color, leftText);
            rightText_UI = new UI_Text(screenPos_X + gap / 2, screenPos_Y, UI_Origin.CENTER_LEFT, font, color, rightText);
        }

        // Draws the UI element.
        public override void Draw_UI()
        {
            leftText_UI.Draw_UI();
            rightText_UI.Draw_UI();
        }
    }
}