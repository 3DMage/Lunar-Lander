using Microsoft.Xna.Framework;
using System;

namespace LunarLander.Assets.ParticleSystems
{
    // A packet of parameters that describes a particle.
    public class ParticleParameters
    {
        // The particle's position, rotation and size.
        public Vector2 position;
        public float radianRotation;
        public Vector2 size;

        // Max lifetime of the particle.
        public TimeSpan maxLifeTime;

        // Velocity of the particle.
        public Vector2 velocity;

        // Color of the particle.
        public Color color;

        // Opacity of the particle.
        public float opacity;

        // Constructor.
        public ParticleParameters(Vector2 position, float radianRotation, Vector2 size, TimeSpan maxLifeTime, Vector2 velocity, Color color, float opacity)
        {
            this.position = position;
            this.radianRotation = radianRotation;
            this.size = size;
            this.maxLifeTime = maxLifeTime;
            this.velocity = velocity;
            this.color = color;
            this.opacity = opacity;
        }
    }
}
