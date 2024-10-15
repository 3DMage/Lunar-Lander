using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Graphics
{
    // Holds data necessary for drawing for all gamestates.
    public class GraphicsManager
    {
        // Drawing component
        public GraphicsDeviceManager graphicsDeviceManager { get; private set; }
        public GraphicsDevice graphicsDevice { get; private set; }
        public SpriteBatch spriteBatch { get; private set; }

        // Window rendering
        public int WINDOW_WIDTH { get; private set; }
        public int WINDOW_HEIGHT { get; private set; }

        // Bounds of the screen.
        public Vector2 bounds { get; private set; }

        // Fonts
        public SpriteFont spaceFont { get; private set; }
        public SpriteFont spaceFontHeader { get; private set; }

        // Textures
        public Texture2D titleLogo { get; private set; }
        public Texture2D landerTexture { get; private set; }
        public Texture2D collisionCircleTexture { get; private set; }
        public Texture2D rectColorTexture { get; private set; }
        public Texture2D backgroundTexture { get; private set; }
        public Texture2D menuBackgroundTexture { get; private set; }
        public Texture2D particle1Texture { get; private set; }

        // Constructor.
        public GraphicsManager(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice)
        {
            // Setup core graphics components.
            this.graphicsDeviceManager = graphicsDeviceManager;
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(this.graphicsDevice);

            // Initialize window rendering properties
            WINDOW_WIDTH = 1920;
            WINDOW_HEIGHT = 1080;

            bounds = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        }

        // Load textures from Content.
        private void LoadTextures()
        {
            titleLogo = ContentManagerHandle.Content.Load<Texture2D>("sprites/titleLogo");
            landerTexture = ContentManagerHandle.Content.Load<Texture2D>("sprites/lander");
            collisionCircleTexture = ContentManagerHandle.Content.Load<Texture2D>("sprites/collisionCircle");
            rectColorTexture = ContentManagerHandle.Content.Load<Texture2D>("sprites/rectColor");
            backgroundTexture = ContentManagerHandle.Content.Load<Texture2D>("sprites/background");
            menuBackgroundTexture = ContentManagerHandle.Content.Load<Texture2D>("sprites/menuBackground");
            particle1Texture = ContentManagerHandle.Content.Load<Texture2D>("sprites/particle1");
        }

        // Sets back buffer width and height, and configures fullscreen mode.
        public void ConfigureGraphicsDevice()
        {
            // Set back buffer dimensions
            graphicsDeviceManager.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphicsDeviceManager.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphicsDeviceManager.ToggleFullScreen();

            // Apply these settings to the graphics device manager.
            graphicsDeviceManager.ApplyChanges();

            graphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.CullCounterClockwiseFace,
                MultiSampleAntiAlias = true,
            };
        }

        // Loads all textures from content.
        public void LoadGraphicsAssets()
        {
            LoadTextures();
            LoadFonts();
            LoadEffects();
        }

        // Load fonts from Content.
        private void LoadFonts()
        {
            spaceFont = ContentManagerHandle.Content.Load<SpriteFont>("fonts/spaceFont");
            spaceFontHeader = ContentManagerHandle.Content.Load<SpriteFont>("fonts/spaceFontHeader");
        }

        // Load effects.
        private void LoadEffects()
        {
            // Load rendering effects here.
        }
    }
}