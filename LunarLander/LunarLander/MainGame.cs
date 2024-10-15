using Audio;
using Scenes;
using Graphics;
using Input;
using LunarLander.Scenes.Scenes;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using LunarLander.Assets.GameData;

namespace LunarLander
{
    // Main game class.
    public class MainGame : Game
    {
        // Managers for different aspects of the game.
        private SceneManager sceneManager;
        private GraphicsDeviceManager graphicsDeviceManager;
        private GraphicsManager graphicsManager;
        private InputManager inputManager;
        private AudioManager audioManager;
        private GameDataManager gameDataManager;


        // Initialize foundational settings for game.
        public MainGame()
        {
            // Grab handles to allow state classes to access certain functionalities.
            ContentManagerHandle.Content = Content;
            GameHandle.game = this;

            // Initialize core components.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Initializtion method.
        protected override void Initialize()
        {
            // Initialize managers.
            sceneManager = new SceneManager();
            graphicsManager = new GraphicsManager(graphicsDeviceManager, GraphicsDevice);
            inputManager = new InputManager();
            audioManager = new AudioManager();
            gameDataManager = new GameDataManager();

            Managers.InjectManagers(sceneManager, graphicsManager, inputManager, audioManager, gameDataManager);

            Managers.gameDataManager.TryLoadingScoresList();
            Managers.inputManager.TryLoadingInputMap();

            // Configure screen settings.
            Managers.graphicsManager.ConfigureGraphicsDevice();

            base.Initialize();
        }

        // Loading method.
        protected override void LoadContent()
        {
            // Load assets.
            graphicsManager.LoadGraphicsAssets();
            audioManager.LoadAudio();

            // Configure audio.
            audioManager.ConfigureAudio();

            // Set up game states.
            sceneManager.AddScene(SceneLabel.MAIN_MENU, new Sc_Menu());
            sceneManager.AddScene(SceneLabel.GAME, new Sc_LunarGame());

            sceneManager.InitializeScenes();

            // Mark initial game state.
            sceneManager.TransitionScene(SceneLabel.MAIN_MENU);
        }

        // Update method.
        protected override void Update(GameTime gameTime)
        {
            // Process input and perform updates for current game state.
            sceneManager.currentGameState.Update(gameTime);

            base.Update(gameTime);
        }

        // Draw method.
        protected override void Draw(GameTime gameTime)
        {
            // Draw current game state.
            
            sceneManager.currentGameState.Draw();

            base.Draw(gameTime);
        }
    }
}