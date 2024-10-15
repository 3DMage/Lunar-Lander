using Microsoft.Xna.Framework;

namespace LunarLander.Physics
{
    // Collider represented using a circle.
    public class CircleCollider
    {
        // Position of the collider.
        public Vector2 position { get; set; }

        // Radius of circle collider.
        public float radius { get; set; }

        // Constructor.
        public CircleCollider(Vector2 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
}
