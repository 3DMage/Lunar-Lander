using LunarLander.DataManagement;
using LunarLander.Input;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Input
{
    // Handles the setup and execution of user input and commands.  Also handles typing.
    public class InputManager
    {
        // Holds the mapping of keys and commands.
        public InputMap inputMap;

        // Persistant storage management of inputs.
        public InputMap_IO inputMap_IO { get; private set; }

        // Flag indicating if an input map was loaded in by InputMap_IO.
        public bool loadedPreexistingInputMap = false;

        // Flag to indicate if the user is done typing.
        public bool doneTyping = false;

        // The current context from which inputs are selected from.
        public InputContext currentInputContext { get; private set; }

        // Stores the current and previous states of the keyboard.
        public KeyboardState currentState { get; private set; }
        public KeyboardState previousState { get; private set; }

        // Collects keys that are pressed to then execute.
        public Keys[] keysBuffer { get; private set; }

        // String used to store typing output.
        public string workingString { get; set; }
        private int characterLimit;

        // Characters obtained by holding Shift key and a number key.
        private string[] shiftNumberCharacters = { ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };

        // A flag that globally marks if any key was pressed.
        private bool globalWasPressed = false;

        // The latest key to be pressed.
        public Keys latestPressedKey;

        // Constructor.
        public InputManager()
        {
            inputMap = null;
        }

        // Try loading an InputMap from persistent storage.  Initialize a new one if no InputMap could be loaded.
        public void TryLoadingInputMap()
        {
            inputMap_IO = new InputMap_IO();

            inputMap_IO.LoadInputMap();

            if (inputMap != null)
            {
                // InputMap was successfully loaded.
                loadedPreexistingInputMap = true;
            }
            else
            {
                // No InputMap was found.  Initialize a new one. 
                loadedPreexistingInputMap = false;
                inputMap = new InputMap();
            }
        }

        // Processes commands associated with pressed keys.
        public void ProcessCommands(GameTime gameTime)
        {
            // Updates current keyboard state.
            currentState = Keyboard.GetState();

            // Grab pressed keys.
            keysBuffer = currentState.GetPressedKeys();


            // Process commands based on stored pressed keys.
            for (int i = 0; i < keysBuffer.Length; i++)
            {
                // Grab a pressed key.
                Keys key = keysBuffer[i];

                // Check if command has entry in key map.
                if (inputMap.keyMap.ContainsKey((currentInputContext, key)))
                {
                    // Grab the inputMapEntry from input map.
                    CommandPacket commandPacket = inputMap.commandPackets[inputMap.keyMap[(currentInputContext, key)]];

                    // Check if command is press-only/
                    if (commandPacket.commandMode == CommandMode.PRESS_ONLY && commandPacket.pressCommand != null)
                    {
                        // Key is press-only.

                        // Check if key can be pressed.
                        if (CanPressKey(key))
                        {
                            // Execute command.
                            commandPacket.pressCommand(gameTime);
                        }
                    }
                    else if (commandPacket.commandMode == CommandMode.PRESS_REGULAR && commandPacket.pressCommand != null)
                    {
                        // Key is not press-only.

                        // Check if key is currently down.
                        if (currentState.IsKeyDown(key))
                        {
                            // Execute command.
                            commandPacket.pressCommand(gameTime);
                        }
                    }
                }
            }

            // Process RELEASE ONLY keys.
            foreach (var key in previousState.GetPressedKeys())
            {
                if (inputMap.keyMap.ContainsKey((currentInputContext, key)))
                {
                    // Grab the command packet from input map.
                    CommandPacket commandPacket = inputMap.commandPackets[inputMap.keyMap[(currentInputContext, key)]];

                    if (commandPacket.releaseCommand != null)
                    {
                        // Check if key was released.

                        if (WasReleasedKey(key))
                        {
                            // Execute command.                
                            commandPacket.releaseCommand(gameTime);
                        }
                    }
                }
            }

            // Mark globalWasPressed flag.
            if (currentState.GetPressedKeyCount() > 0)
            {
                // Some keys are pressed.
                globalWasPressed = true;
            }
            else
            {
                globalWasPressed = false;
            }

            // Updates previous keyboard state.
            previousState = currentState;
        }

        // Updates the currentInputContext.
        public void TransitionInputContext(InputContext inputContext)
        {
            this.currentInputContext = inputContext;
        }

        // Updates an entry in input map based on pressed key.
        public ConfigState ConfigKey(InputContext inputContext, Keys oldKey)
        {
            // Updates current keyboard state.
            currentState = Keyboard.GetState();

            // Grab pressed keys.
            keysBuffer = currentState.GetPressedKeys();

            // Only process if no keys are pressed previously and some key is pressed.
            if (!globalWasPressed && keysBuffer.Length > 0)
            {
                // Get the first key that was pressed in keysBuffer.
                Keys firstPressedKey = Managers.inputManager.keysBuffer[0];

                // Update latestPressedKey.
                latestPressedKey = firstPressedKey;

                // Update globalWasPressed flag.
                globalWasPressed = true;

                // See if input map entry exists for pressed key and is not Escape.
                if (!inputMap.keyMap.ContainsKey((inputContext, latestPressedKey)) && latestPressedKey != Keys.Escape)
                {
                    // Remap the key if the pressed key was not mapped previously.
                    inputMap.RemapKey(inputContext, oldKey, latestPressedKey);

                    return ConfigState.VALID;
                }
                else
                {
                    if (latestPressedKey == oldKey)
                    {
                        // Key was already pressed previously.
                        return ConfigState.ITSELF;
                    }
                    else if (latestPressedKey == Keys.Escape)
                    {
                        // Key is Escape.  This indicates canceling.
                        return ConfigState.CANCELED;
                    }
                    else
                    {
                        // Key was already mapped elsewhere.
                        return ConfigState.ALREADY_MAPPED;
                    }
                }
            }

            // Update globalWasPressed to false if no keys are currently pressed.
            if (currentState.GetPressedKeyCount() <= 0)
            {
                globalWasPressed = false;
            }

            // Updates previous keyboard state.
            previousState = currentState;

            return ConfigState.NOTHING;
        }

        // Sets limit of how many characters can be typed.
        public void SetCharacterLimit(int characterLimit)
        {
            this.characterLimit = characterLimit;
        }

        // Records keys while typing.
        public void GetTypedCharacters()
        {
            // Updates current keyboard state.
            currentState = Keyboard.GetState();

            // Get pressed keys.
            keysBuffer = currentState.GetPressedKeys();

            // Check if shift and caps lock are pressed or enabled respectively.
            bool shiftDown = currentState.IsKeyDown(Keys.LeftShift) || currentState.IsKeyDown(Keys.RightShift);
            bool capitalMode = shiftDown || currentState.CapsLock;

            // Go through each key and insert pressed button into the working string.
            for (int i = 0; i < keysBuffer.Length; i++)
            {
                // Grab current key.
                Keys key = keysBuffer[i];

                // Check if key can be pressed.
                if (CanPressKey(key) && !doneTyping)
                {
                    // Check if backspace is pressed and string is non-empty.
                    if (key == Keys.Back && workingString.Length > 0)
                    {
                        // Backspace pressed. Remove last character from string.
                        workingString = workingString.Substring(0, workingString.Length - 1);
                    }
                    else if (key == Keys.Enter)
                    {
                        // Indicate typing is done.
                        doneTyping = true;
                    }
                    else
                    {
                        // Check if the string's length does not exceed character limit.
                        if (workingString.Length < characterLimit)
                        {
                            // Add character to string.
                            workingString += KeyToString(key, shiftDown, capitalMode);
                        }
                    }
                }
            }

            // Updates previous keyboard state.
            previousState = currentState;
        }

        // Converts pressed key into a string character.  Also checks for shift and capitalization.
        private string KeyToString(Keys key, bool shift, bool capitalMode)
        {
            // Check if key is in number row.
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                // Check if shift is held.
                if (shift)
                {
                    // Grab shift character from number row corresponding to number pressed.
                    return shiftNumberCharacters[key - Keys.D0];
                }

                // Grab the number from number row.
                return ((int)key - (int)Keys.D0).ToString();
            }

            // Check if key is a letter
            if (key >= Keys.A && key <= Keys.Z)
            {
                // Determine if letter is captalized or not.
                if (!capitalMode)
                {
                    return key.ToString().ToLower();
                }
                else
                {
                    return key.ToString().ToUpper();
                }
            }

            // Characters and puncuation.
            switch (key)
            {
                case Keys.Space: return " ";
                case Keys.OemTilde: return shift ? "~" : "`";
                case Keys.OemSemicolon: return shift ? ":" : ";";
                case Keys.OemQuotes: return shift ? "\"" : "'";
                case Keys.OemQuestion: return shift ? "?" : "/";
                case Keys.OemPlus: return shift ? "+" : "=";
                case Keys.OemPipe: return shift ? "|" : "\\";
                case Keys.OemPeriod: return shift ? ">" : ".";
                case Keys.OemOpenBrackets: return shift ? "{" : "[";
                case Keys.OemCloseBrackets: return shift ? "}" : "]";
                case Keys.OemMinus: return shift ? "_" : "-";
                case Keys.OemComma: return shift ? "<" : ",";
                case Keys.OemClear: return shift ? "Clear" : "Clear";
            }

            // No valid key was pressed.
            return "";
        }

        // Clears keyboard states.
        public void ResetKeyboardState()
        {
            currentState = Keyboard.GetState();
            previousState = currentState;
        }

        // Checks if a key can be pressed or not.
        private bool CanPressKey(Keys key)
        {
            return currentState.IsKeyDown(key) && previousState.IsKeyUp(key);
        }

        // Check if key was released.
        private bool WasReleasedKey(Keys key)
        {
            return currentState.IsKeyUp(key) && previousState.IsKeyDown(key);
        }
    }
}