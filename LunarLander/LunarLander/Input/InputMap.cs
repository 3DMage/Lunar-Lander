using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LunarLander.Input
{
    // Holds mapping for keys and commands.
    [DataContract(Name = "InputMap")]
    public class InputMap
    {
        // Dictionary that maps input to names of commands (due to not being able to serialize delegates).
        [DataMember()]
        public Dictionary<(InputContext, Keys), string> keyMap { get; private set; }

        // Stores mapping between command names and CommandPackets.
        public Dictionary<string, CommandPacket> commandPackets { get; private set; }

        // Constructor.
        public InputMap()
        {
            keyMap = new Dictionary<(InputContext, Keys), string>();
            commandPackets = new Dictionary<string, CommandPacket>();
        }

        // Maps a command name to a CommandPacket.
        public void RegisterCommandPacket(string commandPacketName, CommandPacket commandPacket)
        {
            if (commandPackets != null)
            {
                // Map a name to a CommandPacket.
                commandPackets.Add(commandPacketName, commandPacket);
            }
            else
            {
                // Initialize the commandPackets dictionary if not already initialized. Map a name to a CommandPacket.
                commandPackets = new Dictionary<string, CommandPacket>();
                commandPackets.Add(commandPacketName, commandPacket);
            }
        }

        // Registers a key and gamestate to a provided InputMapEntry.  Also specifies if the key is press-only or not.
        public void RegisterKey(Keys key, InputContext inputContext, string commandPacketName)
        {
            keyMap.Add((inputContext, key), commandPacketName);
        }

        // Unregisters a command associated with input key and gamestate.
        public void UnregisterKey(Keys key, InputContext inputContext)
        {
            keyMap.Remove((inputContext, key));
        }

        // Remaps a command to specified key and game state.
        public bool RemapKey(InputContext inputContext, Keys oldKey, Keys newKey)
        {
            // Check if the new key is not already mapped.
            if (!keyMap.ContainsKey((inputContext, newKey)))
            {
                // Do a remap.
                CommandPacket remappedCommandPacket = commandPackets[keyMap[(inputContext, oldKey)]];
                UnregisterKey(oldKey, inputContext);
                RegisterKey(newKey, inputContext, remappedCommandPacket.commandName);
                return true;
            }
            else
            {
                // Can't remap.
                return false;
            }
        }
    }
}