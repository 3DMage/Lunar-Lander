using Microsoft.Xna.Framework.Content;

namespace LunarLander.Utilities
{
    // Handle to Content Manager.
    static class ContentManagerHandle
    {
        // A handle into the Content Manager object for other classes to access.
        public static ContentManager Content { get; set; }
    }
}
