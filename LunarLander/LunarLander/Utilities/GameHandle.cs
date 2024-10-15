using Microsoft.Xna.Framework;

namespace LunarLander.Utilities
{
    // A handle into the main Game object for other classes to access.
    static class GameHandle
    {
        public static Game game { get; set; }
    }
}
