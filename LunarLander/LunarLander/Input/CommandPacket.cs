using Microsoft.Xna.Framework;

// Delegates for function signatures that constitute CommandPacket methods.
public delegate void PressCommand(GameTime gameTime);
public delegate void ReleaseCommand(GameTime gameTime);

namespace LunarLander.Input
{
    // Represents the functionality for a key's press and release.
    public class CommandPacket
    {
        // Function to use when key is pressed.
        public PressCommand pressCommand { get; private set; } = null;

        // Function to use when key is released.
        public ReleaseCommand releaseCommand { get; private set; } = null;

        // How the key's commands should be executed.
        public CommandMode commandMode { get; private set; }

        // Name of the command.
        public string commandName { get; private set; }

        // Constructor.
        public CommandPacket(CommandMode commandMode, PressCommand pressCommand, ReleaseCommand releaseCommand, string commandName)
        {
            this.commandMode = commandMode;
            this.pressCommand = pressCommand;
            this.releaseCommand = releaseCommand;
            this.commandName = commandName;
        }
    }
}
