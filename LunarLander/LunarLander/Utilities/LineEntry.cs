namespace LunarLander.Entities.GameObjects
{
    // A data container representing an entry in a line list for terrain and collision.
    public class LineEntry
    {
        // Extremities of the line.
        public VertexEntry leftVertex { get; private set; }
        public VertexEntry rightVertex { get; private set; }

        // Flag indicating if a line is a safe zone.
        public bool isSafeZone { get; private set; }

        // Constructor.
        public LineEntry(VertexEntry leftVertex, VertexEntry rightVertex, bool isSafeZone)
        {
            this.leftVertex = leftVertex;
            this.rightVertex = rightVertex;
            this.isSafeZone = isSafeZone;
        }
    }
}
