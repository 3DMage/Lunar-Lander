using Audio;
using Scenes;
using Graphics;
using Input;
using LunarLander.Assets.GameData;

namespace LunarLander.Utilities
{
    // Class that holds references to various managers.
    public class Managers
    {
        // Managers.
        public static GraphicsManager graphicsManager;
        public static InputManager inputManager;
        public static AudioManager audioManager;
        public static SceneManager sceneManager;
        public static GameDataManager gameDataManager;

        // Injects references of managers into the Manager class. 
        public static void InjectManagers(SceneManager sceneManager, GraphicsManager graphicsManager, InputManager inputManger, AudioManager audioManager, GameDataManager gameDataManager)
        {
            Managers.sceneManager = sceneManager;
            Managers.inputManager = inputManger;
            Managers.audioManager = audioManager;
            Managers.graphicsManager = graphicsManager;
            Managers.gameDataManager = gameDataManager;
        }
    }
}
