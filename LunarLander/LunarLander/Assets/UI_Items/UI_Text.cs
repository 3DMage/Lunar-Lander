using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LunarLander.Entities.UI_Items
{
    // UI element to show text.
    public class UI_Text : UI_Item
    {
        // Font.
        private SpriteFont font;

        // Text.
        public string text { get; private set; }

        // Text to display.
        private string displayText = "";

        // Timer for state of blinker display if typing.
        private TimeSpan blinkerTimer = new TimeSpan(0);

        // How frequently to blink the blinker for typing.
        private TimeSpan blinkIntervalMS = new TimeSpan(0, 0, 0, 0, 500);

        // Toggle to indicate whether or not to draw the blinker.
        private bool drawBlinker = false;

        // Character to display as the blinker.
        private string blinkerCharacter = "|";

        // Offset to help position blinker when no text is present.
        private Vector2 blankOriginOffset = new Vector2(0, 0);

        // Constructor.
        public UI_Text(int screenPos_X, int screenPos_Y, UI_Origin origin, SpriteFont font, Color color, string text) : base(screenPos_X, screenPos_Y, origin, color)
        {
            this.font = font;
            this.color = color;
            this.text = text;
            displayText = text;

            CalculateOriginOffset(text);
            CalculateBlinkerOffset();
        }

        // Sets the text and display text.
        public void SetText(string text)
        {
            this.text = text;
            this.displayText = text;
            CalculateOriginOffset(displayText);
        }


        // Updates origin point when text changes.
        private void CalculateOriginOffset(string text)
        {
            Vector2 textDimensions = this.font.MeasureString(text);

            if (this.originType == UI_Origin.CENTER)
            {
                originOffset = new Vector2(textDimensions.X / 2, textDimensions.Y / 2);
            }
            else if (this.originType == UI_Origin.UPPER_CENTER)
            {
                originOffset = new Vector2(textDimensions.X / 2, 0);
            }
            else if (this.originType == UI_Origin.BOTTOM_CENTER)
            {
                originOffset = new Vector2(textDimensions.X / 2, textDimensions.Y);
            }
            else if (this.originType == UI_Origin.CENTER_LEFT)
            {
                originOffset = new Vector2(0, textDimensions.Y / 2);
            }
            else if (this.originType == UI_Origin.BOTTOM_LEFT)
            {
                originOffset = new Vector2(0, textDimensions.Y);
            }
            else if (this.originType == UI_Origin.UPPER_RIGHT)
            {
                originOffset = new Vector2(textDimensions.X, 0);
            }
            else if (this.originType == UI_Origin.CENTER_RIGHT)
            {
                originOffset = new Vector2(textDimensions.X, textDimensions.Y / 2);
            }
            else
            {
                originOffset = new Vector2(textDimensions.X, textDimensions.Y);
            }
        }

        // Calculates blinker origin offset for when no text is present.
        private void CalculateBlinkerOffset()
        {
            Vector2 textDimensions = this.font.MeasureString("AAAA");
            blankOriginOffset = new Vector2(0, textDimensions.Y / 2);
        }

        // Draws the UI element.
        public override void Draw_UI()
        {
            Managers.graphicsManager.spriteBatch.DrawString(font, displayText, position, color, 0, originOffset, Vector2.One, SpriteEffects.None, 0);
        }

        // Updates the state of displaying the blinker.
        public void UpdateBlinker(GameTime gameTime)
        {
            blinkerTimer += gameTime.ElapsedGameTime;

            if (blinkerTimer >= blinkIntervalMS)
            {
                drawBlinker = !drawBlinker;
                blinkerTimer = TimeSpan.Zero;
            }

            if (drawBlinker)
            {
                if (displayText == "")
                {
                    originOffset = blankOriginOffset;
                }
                displayText = text + blinkerCharacter;

            }
            else
            {
                displayText = text;
            }
        }

        // Clears the blinker display.
        public void ClearBlinker()
        {
            displayText = text;
        }
    }
}