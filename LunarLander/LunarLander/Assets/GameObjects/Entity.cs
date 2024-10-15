using LunarLander.Entities.UI_Items;
using Microsoft.Xna.Framework;
using System;

namespace LunarLander.GamePieces.Entities.GameObjects
{
    // The base class for all Entity type objects.
    public abstract class Entity
    {
        // Label indicating position of the origin relative to object.
        protected UI_Origin origin { get; private set; }

        // The Entity's position, size and rotation data.
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public float radianRotation { get; set; }

        // The Entity's allowable moving space.
        public Vector2 spaceBounds { get; set; }

        // Constructor.
        protected Entity(Vector2 position, float radianRotation, Vector2 size)
        {
            this.position = position;
            this.radianRotation = radianRotation;
            this.size = size;

            origin = UI_Origin.CENTER;
            spaceBounds = new Vector2(size.X, size.Y);
        }

        // Returns the radian rotation in terms of degrees.
        public float DegreeRotation()
        {
            return (float)(radianRotation * 180.0f / Math.PI);
        }
    }
}
