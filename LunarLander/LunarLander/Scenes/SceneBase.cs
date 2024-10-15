using LunarLander.Input;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;

namespace Scenes
{
    // Update method delegate.
    public delegate void Internal_Update(GameTime gameTime);

    // Draw method delegate.
    public delegate void Internal_Draw();

    // The base class that represents a game state.
    public abstract class SceneBase
    {
        // Delegates for update and drawing.
        protected Internal_Update updateDelegate;
        protected Internal_Draw drawDelegate;

        // Current context to select input with.
        public InputContext inputContext { get; private set; }

        // Current way to handle user input.
        public InputMode inputMode { get; private set; }

        // Calls the current draw delegate method.
        public void Draw()
        {
            drawDelegate();
        }

        // Calls the current update delegate method.
        public void Update(GameTime gameTime)
        {
            updateDelegate(gameTime);
        }

        // Initialization.  Called when state added the GameStateManager.
        public abstract void Initialize();

        // Abstract method to register default keys when InputMap is not loaded via persistent storage.
        public abstract void RegisterDefaultKeys();

        // Abstract method to register scene CommandPackets.
        public abstract void RegisterCommandPackets();

        // Updates state update and drawing methods, as well as input context and input mode.
        public void TransitionState(Internal_Update updateMethod, Internal_Draw drawMethod, InputContext inputContext, InputMode inputMode)
        {
            updateDelegate = updateMethod;
            drawDelegate = drawMethod;
            this.inputMode = inputMode;
            this.inputContext = inputContext;
            Managers.inputManager.TransitionInputContext(inputContext);
            Managers.inputManager.ResetKeyboardState();
        }

        // Called when the scene is entered into.
        public virtual void EnterState() { }

        // Called when the scene is exited.
        public virtual void ExitState() { }
    }
}