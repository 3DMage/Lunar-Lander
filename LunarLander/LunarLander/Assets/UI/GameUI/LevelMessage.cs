using LunarLander.Entities.UI_Items;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace LunarLander.Assets.Menu.GameUI
{
    // The message that is displayed at start and end of level.
    public class LevelMessage
    {
        // The UI element holding the message text.
        UI_Text message_UI;

        // Constructor.
        public LevelMessage()
        {
            message_UI = new UI_Text
            (
                Managers.graphicsManager.WINDOW_WIDTH / 2,
                Managers.graphicsManager.WINDOW_HEIGHT / 2,
                UI_Origin.CENTER,
                Managers.graphicsManager.spaceFontHeader,
                Color.White,
                ""
            );
        }

        // Updates text of the message.
        public void UpdateMessageText(string messageText)
        {
            message_UI.SetText(messageText);
        }

        // Updates color of the message.
        public void UpdateColor(Color color)
        {
            message_UI.color = color;
        }

        // Draws the message.
        public void Draw()
        {
            message_UI.Draw_UI();
        }
    }
}
