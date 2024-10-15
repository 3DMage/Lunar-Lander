using LunarLander.GamePieces.Entities.GameObjects;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LunarLander.Assets.ParticleSystems
{
    // Base class for Emitter objects.
    public abstract class Emitter : Entity
    {
        // Max number of particles that the emitter can emit.
        public int maxParticleCount;

        // List of particles.
        public Particle[] particles;

        // List of that can be spawned.
        public Stack<int> vacantParticleIndices;

        // Texture to use for each particle from the emitter.
        public Texture2D particleTexture;

        // Constructor.
        public Emitter(Vector2 position, float radianRotation, int maxParticleCount) : base(position, radianRotation, new Vector2(0, 0))
        {
            this.maxParticleCount = maxParticleCount;
            particles = new Particle[maxParticleCount];
            vacantParticleIndices = new Stack<int>(maxParticleCount);

            // Fill particles array with blank particles.
            for (int i = 0; i < maxParticleCount; i++)
            {
                particles[i] = new Particle();
                vacantParticleIndices.Push(i);
            }

            // Add emitter to the Particle System.
            ParticleSystem.AddEmitter(this);
        }

        // Updates all alive particles.
        public void UpdateParticles(GameTime gameTime)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isAlive)
                {
                    particles[i].Update(gameTime);

                    if (!particles[i].isAlive)
                    {
                        vacantParticleIndices.Push(i);
                    }
                }
            }
        }

        // Clears all active particles.
        public void ClearParticles()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].isAlive = false;
            }
        }

        // Draws all alive particles.
        public virtual void DrawParticles()
        {
            Managers.graphicsManager.spriteBatch.Begin();
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isAlive)
                {
                    particles[i].Draw();
                }
            }
            Managers.graphicsManager.spriteBatch.End();
        }
    }
}
