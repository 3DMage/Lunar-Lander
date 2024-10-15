using LunarLander.GamePieces.Entities.GameObjects;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LunarLander.Assets.ParticleSystems
{
    // Represents a particle in the world.
    public class Particle : Entity
    {
        // Velocity of the particle.
        private Vector2 velocity;

        // Max lifetime possible for the particle.
        private TimeSpan maxLifetime;

        // Current time the particle is active.
        private TimeSpan currentLifeTime;

        // Texture used by the particle.
        protected Texture2D particleTexture;

        // Offset factor to position origin relative to object's texture.
        protected Vector2 originOffset;

        // A bool indicating if the particle is alive or not.
        public bool isAlive;

        // Offset factor to size the rendering of the particle relative to given size of particle.
        public Vector2 renderScaleFactor { get; private set; }

        // Opacity of the particle.
        public float opacity;

        // Color of the particle.
        public Color color;

        // Constructor.
        public Particle() : base(new Vector2(0, 0), 0, new Vector2(0, 0)) { }

        // Initiates particle with Particle Parameters object and a texture.
        public void InitiateParticle(ParticleParameters parameters, Texture2D particleTexture)
        {
            this.position = parameters.position;
            this.radianRotation = parameters.radianRotation;
            this.size = parameters.size;
            this.velocity = parameters.velocity;
            this.maxLifetime = parameters.maxLifeTime;
            this.currentLifeTime = TimeSpan.Zero;
            this.particleTexture = particleTexture;
            this.isAlive = true;
            this.opacity = parameters.opacity;
            this.color = parameters.color;

            // Compute origin offset to be in center of texture.
            originOffset = new Vector2(particleTexture.Width / 2f, particleTexture.Height / 2f);

            // Compute render scale factor.
            float renderScaleFactorX = (float)(size.X / particleTexture.Width);
            float renderScaleFactorY = (float)(size.Y / particleTexture.Height);
            renderScaleFactor = new Vector2(renderScaleFactorX, renderScaleFactorY);
        }

        // Updates the particle.
        public virtual void Update(GameTime gameTime)
        {
            position += velocity;
            currentLifeTime += gameTime.ElapsedGameTime;

            // Kill particle if it's current life time exceeds max life time.
            isAlive = currentLifeTime < maxLifetime;
        }

        // Draw the particle.
        public virtual void Draw()
        {
            Managers.graphicsManager.spriteBatch.Draw(particleTexture, position, null, color, radianRotation, originOffset, renderScaleFactor, SpriteEffects.None, 0f);
        }
    }
}