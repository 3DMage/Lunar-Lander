using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LunarLander.Assets.ParticleSystems
{
    // The particle system.  Manages all emitters attached to it.
    public static class ParticleSystem
    {
        // List of emitters used by the game.
        public static List<Emitter> emitters = new List<Emitter>();

        // Adds a new emitter to the emitters list.
        public static void AddEmitter(Emitter emitter)
        {
            emitters.Add(emitter);
        }

        // Adds a new emitter to the emitters list.
        public static void RemoveEmitter(Emitter emitter)
        {
            emitters.Remove(emitter);
        }

        // Update state of emitters.  This include both emission and particle updates.
        public static void UpdateEmitters(GameTime gameTime)
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].UpdateParticles(gameTime);
            }
        }

        // Draw particles from each emitter.
        public static void DrawEmitterParticles()
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].DrawParticles();
            }
        }

        // Clears all particles for all emitters.
        public static void ClearParticles()
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].ClearParticles();
            }
        }
    }
}
