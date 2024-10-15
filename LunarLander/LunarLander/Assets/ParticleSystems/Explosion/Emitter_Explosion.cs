using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LunarLander.Assets.ParticleSystems.Explosion
{
    // Emitter that spawns an explosion effect.
    public class Emitter_Explosion : Emitter
    {
        // Random object.
        Random random = new Random();

        // Max lifetime of the particles.
        float maxLifeTimeSeconds = 2.0f;

        // Causes particles to use additive blending to simulate lighting when particles overlap.
        BlendState additiveBlend = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            AlphaBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One
        };

        // Constructor.
        public Emitter_Explosion(Vector2 position, float radianRotation, int maxParticleCount) : base(position, radianRotation, maxParticleCount)
        {
            particleTexture = Managers.graphicsManager.particle1Texture;
        }

        // Makes all particles in a circle for explosion effect.
        public void ShipExplosion()
        {
            for (int i = 0; i < maxParticleCount; i++)
            {
                ParticleParameters parameters = MakeRingOfFireComponent(i);
                if (vacantParticleIndices.Count > 0)
                {
                    particles[vacantParticleIndices.Pop()].InitiateParticle(parameters, particleTexture);
                }
            }
        }

        // Make the particle parameters needed to simulate a ring of fire.
        private ParticleParameters MakeRingOfFireComponent(int currentParticleIndex)
        {
            // Keep the start size of the particle as is or adjust according to your needs
            float startSize = random.Next(25, 56);

            // Set a constant, high speed for the "ring of fire" effect
            float speed = 5.0f + (float)random.NextDouble() * 5.0f; // Generates a random float between 5 and 10

            // Distribute particles uniformly in a circle
            float angleStep = (float)(Math.PI * 2 / maxParticleCount); // Full circle divided by the number of particles
            float currentAngle = angleStep * currentParticleIndex + radianRotation; // Calculate current particle's angle

            // Calculate velocity based on the angle to ensure uniform distribution in a ring
            Vector2 velocity = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)) * speed;

            // Define particle color and opacity
            Color color = Color.Lerp(Color.Yellow, Color.Red, (float)random.NextDouble()); // Gradually changes from yellow to red
            float opacity = 1.0f; // Full opacity

            // Set lifetime to ensure all particles dissipate together, enhancing the ring effect
            TimeSpan lifeTime = TimeSpan.FromSeconds(maxLifeTimeSeconds); // Use the max lifetime for uniformity

            // Use the emitter's position as the starting point for the particle
            Vector2 position = this.position;

            // Set or randomize the size of the particle if needed
            Vector2 size = new Vector2(startSize, startSize);

            // Increment the index for the next particle to be positioned on the ring
            currentParticleIndex = (currentParticleIndex + 1) % maxParticleCount;

            return new ParticleParameters(position, 0, size, lifeTime, velocity, color, opacity);
        }

        // Draws the particles that are alive.  Use additive blend state.
        public override void DrawParticles()
        {
            Managers.graphicsManager.spriteBatch.Begin(SpriteSortMode.Deferred, additiveBlend);
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

