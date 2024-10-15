using LunarLander.Entities.GameObjects;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LunarLander.Assets.ParticleSystems.Thrust
{
    // Emitter for making smoke particles for thruster.
    public class Emitter_Thrust : Emitter
    {
        // Reference to lander.
        Lander lander;

        // Random object.
        Random random = new Random();

        // Angle spread for thruster particles.  About 75 degrees.
        float exhaustSpreadAngle = 1.09f;
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
        public Emitter_Thrust(Vector2 position, float radianRotation, int maxParticleCount, Lander lander) : base(position, radianRotation, maxParticleCount)
        {
            this.lander = lander;
            particleTexture = Managers.graphicsManager.particle1Texture;
        }

        // Creates a new particle.
        public void ShipThrust()
        {
            // Configure the particle.
            ParticleParameters parameters = MakeParticleParameters();

            // If the emitter can emit and there are empty particles, spawn a particle.
            if (vacantParticleIndices.Count > 0)
            {
                particles[vacantParticleIndices.Pop()].InitiateParticle(parameters, particleTexture);
            }
        }

        // Make the particle parameters needed to simulate thruster exhaust.
        private ParticleParameters MakeParticleParameters()
        {
            // Position of where particle should spawn.
            Vector2 startPosition = position;

            // Random size of particle between 5 and 15 for both width and height.
            int particleSize = random.Next(5, 16); // The upper bound in Next is exclusive, hence 16.
            Vector2 size = new Vector2(particleSize, particleSize);

            // Random lifetime between 0.2 seconds and maxLifeTime
            TimeSpan lifeTime = TimeSpan.FromSeconds(random.NextDouble() * (maxLifeTimeSeconds - 0.2) + 0.2); 
            
            // Determine angle offset from emitter.
            float angleOffset = ((float)random.NextDouble() - 0.5f) * exhaustSpreadAngle; 

            // Rotation matrix to rotate emission direction using angle offset.
            Matrix rotationMatrix = Matrix.CreateRotationZ(angleOffset);

            // Compute direction of emission relative to lander rotation.
            Vector2 direction = new Vector2((float)Math.Cos(lander.radianRotation + MathF.PI / 2), (float)Math.Sin(lander.radianRotation + MathF.PI/2));

            // Use rotation matrix to apply angle offset.
            Vector3 exhaustVelocity_TEMP = new Vector3(direction, 0);
            exhaustVelocity_TEMP = Vector3.Transform(exhaustVelocity_TEMP, rotationMatrix);

            // Scale the exhaust velocity by a random factor.
            exhaustVelocity_TEMP *= (float)(random.NextDouble() * 3.0 + 1.0); // Random speed

            // Make final Vector2 exhaust velocity.
            Vector2 exhaustVelocity = new Vector2(exhaustVelocity_TEMP.X, exhaustVelocity_TEMP.Y);

            return new ParticleParameters(startPosition, 0, size, lifeTime, exhaustVelocity, Color.White, 1.0f);
        }

        // Draw all alive particles.  Use additive blend state.
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
