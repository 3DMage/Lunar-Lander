using LunarLander.Utilities;
using System.Collections.Generic;

namespace Scenes
{
    // This stores all possible game states and manages what is the current game state.
    public class SceneManager
    {
        // Holds all the game states.
        public Dictionary<SceneLabel, SceneBase> scenes { get; private set; }

        // The current game state in use.
        public SceneBase currentGameState { get; private set; }

        // Constructor.
        public SceneManager()
        {
            scenes = new Dictionary<SceneLabel, SceneBase>();
        }

        // Adds a new game state to the game states dictonary.
        public void AddScene(SceneLabel gameStateLabel, SceneBase scene)
        {
            scenes.Add(gameStateLabel, scene);
        }

        // Initializes the scene.  All scenes must register any CommandPackets attached to it.
        public void InitializeScenes()
        {
            //? Register command packets FIRST
            foreach (SceneBase scene in scenes.Values)
            {
                scene.RegisterCommandPackets();
            }

            //? Register default inputs if no pre-existing map was loaded.
            if (!Managers.inputManager.loadedPreexistingInputMap)
            {
                foreach (SceneBase scene in scenes.Values)
                {
                    scene.RegisterDefaultKeys();
                }

                Managers.inputManager.inputMap_IO.SaveInputMap();
            }

            //? Intialize each scene's state.
            foreach (SceneBase scene in scenes.Values)
            {
                scene.Initialize();
            }
        }

        // Goes to one game state to another.  If no current game state exists, just enter into the input game state.
        public void TransitionScene(SceneLabel gamestateLabel)
        {
            // Check if a current scene is active.
            if (scenes.ContainsKey(gamestateLabel) && currentGameState != null)
            {
                // Call exit scene and enter state methods in addition to current game state update.
                currentGameState.ExitState();
                currentGameState = scenes[gamestateLabel];
                currentGameState.EnterState();
            }
            else
            {
                // Only call enter state methods in addition to current game state update.
                currentGameState = scenes[gamestateLabel];
                currentGameState.EnterState();
            }
        }
    }
}
