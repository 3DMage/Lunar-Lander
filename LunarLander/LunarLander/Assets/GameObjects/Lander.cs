using LunarLander.Assets.ParticleSystems.Explosion;
using LunarLander.Assets.ParticleSystems.Thrust;
using LunarLander.GamePieces.Entities.GameObjects;
using LunarLander.Physics;
using LunarLander.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LunarLander.Entities.GameObjects
{
    // The main player object in the Lunar Lander game.
    public class Lander : Entity
    {
        // Texture reference for lander.
        public Texture2D landerTexture { get; private set; }

        // Emitter for thrust particle effect.
        public Emitter_Thrust thrustEmitter;

        // Emitter for explosion particle effect.
        public Emitter_Explosion explodeEmitter;

        // Physics collider surrounding the lander object.
        public CircleCollider collider { get; private set; }

        // Gravity reference used on lander.
        public Gravity gravity { get; private set; }

        // The offset to object to make its position relative to the corresponding UI_Origin tag.
        public Vector2 originOffset { get; private set; }

        // An offset factor to scale object based on its size and bounds.
        public Vector2 renderScaleFactor { get; private set; }

        // Initial rotation of the lander on spawn.  In this case, it is about 270 degrees.
        private float initialRotation = (float)(3 * Math.PI / 2);

        // Initial fuel the lander starts with at the start of each level.
        private float initialFuelAmount = 1000.0f;

        // Current amount fuel left on lander object.
        public float fuel;

        // A factor offset to convert ship's screen speed into the speed relevant to the game.
        private float speedConversionFactor = 9.0f;

        // The initial speed of the lander in terms of screen speed.
        private float initialSpeed_NonConverted = 1.2f;

        // How fast the lander spins.
        public float spinRateRadians { get; private set; } = 0.75f;

        // How fast the fuel is used.
        public float fuelUseRate { get; private set; } = 25.0f;

        // How fast the thrust accelerates the ship.
        public float thrustAcceleration { get; private set; } = .65f;

        // Current velocity of the lander.
        public Vector2 currentVelocity { get; set; }

        // Constructor.
        public Lander(Texture2D landerTexture, Gravity gravity, Vector2 initialPosition, Vector2 bounds) : base(initialPosition, (float)(3 * Math.PI / 2), new Vector2(42.0f, 42.0f))
        {
            // Compute render scale factor.
            float renderScaleFactorX = (float)(size.X / landerTexture.Width);
            float renderScaleFactorY = (float)(size.Y / landerTexture.Height);
            renderScaleFactor = new Vector2(renderScaleFactorX, renderScaleFactorY);

            currentVelocity = new Vector2(initialSpeed_NonConverted * 1.0f, 0.0f);
            this.landerTexture = landerTexture;
            this.gravity = gravity;
            collider = new CircleCollider(position, size.X / 2);

            // Compute origin offset based on lander texture size.
            originOffset = new Vector2(landerTexture.Width / 2f, landerTexture.Height / 2f);

            // Compute the bounds of the object.
            this.spaceBounds = new Vector2(bounds.X - collider.radius, bounds.Y);

            fuel = initialFuelAmount;

            thrustEmitter = new Emitter_Thrust(position, radianRotation, 500, this);
            explodeEmitter = new Emitter_Explosion(Vector2.Zero, 0, 1000);
        }

        // Updates position of lander and all subobjects attached to it.
        public void UpdatePosition()
        {
            // Update lander position.
            position += currentVelocity;

            // Prevent lander from going off screen.
            position = new Vector2(Math.Clamp(position.X, collider.radius, spaceBounds.X), Math.Clamp(position.Y, collider.radius, spaceBounds.Y));

            // Update collider position.
            collider.position = position;

            // Update emitter position.
            UpdateThrusterEmitterPosition();
        }

        // Update the rotation of the object at specified direction.
        public void UpdateAngle(bool isClockWise, GameTime elapsedTime)
        {
            // Rotate clockwise
            if (isClockWise)
            {
                // Rotate clockwise
                radianRotation += (float)((spinRateRadians * elapsedTime.ElapsedGameTime.TotalSeconds));
            }
            else
            {
                // Rotate counter-clockwise
                radianRotation += -(float)((spinRateRadians * elapsedTime.ElapsedGameTime.TotalSeconds));
            }

            // Update emitter position based on lander rotation.
            radianRotation = MathKit.modF(radianRotation, (float)(2 * Math.PI));
            UpdateThrusterEmitterPosition();
        }

        // Updates the position of the thruster emitter relative to the lander.
        private void UpdateThrusterEmitterPosition()
        {
            // Calculate the offset for the emitter to be positioned near the bottom border of the lander.

            // Emitter centered on X-axis relative to lander.
            float emitterOffsetX = 0;

            // Emitter positioned at bottom of lander.
            float emitterOffsetY = landerTexture.Height * renderScaleFactor.Y / 2;

            // Calculate the new position.
            Vector2 emitterPosition = position + new Vector2
            (
                emitterOffsetX * (float)Math.Cos(radianRotation) - emitterOffsetY * (float)Math.Sin(radianRotation),
                emitterOffsetX * (float)Math.Sin(radianRotation) + emitterOffsetY * (float)Math.Cos(radianRotation)
            );

            // Update the emitter's position.
            thrustEmitter.position = emitterPosition;

            // Ensure the emitter's rotation matches the lander's rotation.
            thrustEmitter.radianRotation = this.radianRotation;
        }

        // Updates velocity of the lander by adding downward gravity to it.
        public void ApplyGravity(GameTime elapsedTime)
        {
            currentVelocity += new Vector2(0, (float)(gravity.MOON_GRAVITY * elapsedTime.ElapsedGameTime.TotalSeconds));
        }

        // Updates velocity of the lander by adding thrust at direction of lander's current rotation.
        public void ApplyThrust(GameTime elapsedTimeMS)
        {
            // Check if any fuel remains.
            if (fuel > 0)
            {
                // Obtain thrust direction and magnitude.
                float elapsedTimeInSeconds = (float)elapsedTimeMS.ElapsedGameTime.TotalSeconds;
                float x = (float)(thrustAcceleration * elapsedTimeInSeconds * Math.Sin(radianRotation));
                float y = -(float)(thrustAcceleration * elapsedTimeInSeconds * Math.Cos(radianRotation));
                Vector2 thrust = new Vector2(x, y);

                // Update lander velocity.
                currentVelocity += thrust;

                // Update remaining fuel amount.
                fuel -= fuelUseRate * elapsedTimeInSeconds;

                // Make the thrust emitter emit particles.
                thrustEmitter.ShipThrust();

                // Active sound of thrusters if not already playing.
                if (Managers.audioManager.rocketRumble.State != SoundState.Playing)
                {
                    Managers.audioManager.rocketRumble.Play();
                }

                if (fuel <= 0)
                {
                    // Ensure fuel left does not go below 0.
                    fuel = 0;

                    // Stop playing thruster noise.
                    Managers.audioManager.alert.Play();
                    
                    // Stop thrust noise.
                    Managers.audioManager.rocketRumble.Stop();
                }
            }
        }

        // Called when the ship is no longer using thrust.
        public void Unthrust()
        {
            Managers.audioManager.rocketRumble.Stop();
        }

        // Returns the magnitude of lander's current velocity.
        public float CurrentSpeed()
        {
            return (currentVelocity.Length() * speedConversionFactor);
        }

        // Draws the lander.
        public void Draw()
        {
            Managers.graphicsManager.spriteBatch.Draw(landerTexture, position, null, Color.White, radianRotation, originOffset, renderScaleFactor, SpriteEffects.None, 0f);
        }

        public void ExplodeEffect()
        {
            // Calculate the new position.
            Vector2 emitterPosition = position;

            // Update the emitter's position.
            explodeEmitter.position = emitterPosition;

            explodeEmitter.ShipExplosion();
        }

        // Resets the lander to its initial state for when a new level begins.
        public void ResetLander(Vector2 position)
        {
            this.position = position;
            UpdateThrusterEmitterPosition();
            currentVelocity = new Vector2(initialSpeed_NonConverted * 1.0f, 0.0f);
            radianRotation = initialRotation;
            fuel = initialFuelAmount;
        }
    }
}