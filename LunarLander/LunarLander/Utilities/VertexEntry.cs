using Microsoft.Xna.Framework.Graphics;

namespace LunarLander.Entities.GameObjects
{
    // Data container representing a vertex and a vertex type.
    public class VertexEntry
    {
        // Vertex.
        public VertexPositionColor vertex { get; private set; }

        // Vertex type.
        public VertexType vertexType { get; private set; }

        // Constructor.
        public VertexEntry(VertexPositionColor position, VertexType vertexType)
        {
            vertex = position;
            this.vertexType = vertexType;
        }
    }
}
