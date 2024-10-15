using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LunarLander.Entities.UI_Items
{
    // UI item to draw a picture with.
    public class UI_Picture : UI_Item
    {
        // Texture to render.
        public Texture2D picture { get; private set; }

        // Factor to scale object based on a given size.
        private Vector2 renderScaleFactor;

        // Constructor.
        public UI_Picture(int screenPos_X, int screenPos_Y, int sizeX, int sizeY, UI_Origin origin, Texture2D picture, Color color) : base(screenPos_X, screenPos_Y, origin, color)
        {
            this.picture = picture;

            // Compute render scale factor from given size.
            float renderScaleFactorX = sizeX / (float)picture.Width;
            float renderScaleFactorY = sizeY / (float)picture.Height;
            renderScaleFactor = new Vector2(renderScaleFactorX, renderScaleFactorY);

            // Calculate originOffset based on the given UI_Origin.
            if (this.originType == UI_Origin.CENTER)
            {
                originOffset = new Vector2(picture.Width / 2, picture.Height / 2);
            }
            else if (this.originType == UI_Origin.UPPER_CENTER)
            {
                originOffset = new Vector2(picture.Width / 2, 0);
            }
            else if (this.originType == UI_Origin.BOTTOM_CENTER)
            {
                originOffset = new Vector2(picture.Width / 2, picture.Height);
            }
            else if (this.originType == UI_Origin.CENTER_LEFT)
            {
                originOffset = new Vector2(0, picture.Height / 2);
            }
            else if (this.originType == UI_Origin.BOTTOM_LEFT)
            {
                originOffset = new Vector2(0, picture.Height);
            }
            else if (this.originType == UI_Origin.UPPER_RIGHT)
            {
                originOffset = new Vector2(picture.Width, 0);
            }
            else if (this.originType == UI_Origin.CENTER_RIGHT)
            {
                originOffset = new Vector2(picture.Width, picture.Height / 2);
            }
            else
            {
                originOffset = new Vector2(picture.Width, picture.Height);
            }
        }

        // Draws the UI element.
        public override void Draw_UI()
        {
            Managers.graphicsManager.spriteBatch.Draw(picture, position, null, color, 0, originOffset, renderScaleFactor, SpriteEffects.None, 0);
        }
    }
}