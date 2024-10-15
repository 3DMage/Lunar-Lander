using LunarLander.Entities.GameObjects;
using LunarLander.Utilities;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace LunarLander.Physics
{
    // Handles collision for terrain.
    public class TerrainCollisionHandling
    {
        // How many "partitions" to split screen space into.  Helps reduce number of checks against terrain lines.
        public int PARTITION_COUNT = 20;

        // Width of the screen.
        public int WIDTH = Managers.graphicsManager.WINDOW_WIDTH;

        // How long a partition is.
        public int partitionLength { get; private set; }

        // Lines used in collision checks for terrain.
        public List<LineEntry>[] terrainCollision { get; private set; }

        // Injects terrain data from existing terrain object's line list.
        public void InjectTerrainData(List<LineEntry> lineList)
        {
            terrainCollision = new List<LineEntry>[PARTITION_COUNT + 1];
            partitionLength = WIDTH / PARTITION_COUNT;

            for (int i = 0; i < terrainCollision.Length; i++)
            {
                terrainCollision[i] = new List<LineEntry>();
            }

            for (int i = 0; i < lineList.Count; i++)
            {
                LineEntry currentLine = lineList[i];
                int leftIndex = (int)(currentLine.leftVertex.vertex.Position.X / partitionLength);
                int rightIndex = (int)(currentLine.rightVertex.vertex.Position.X / partitionLength);

                for (int k = leftIndex; k <= rightIndex; k++)
                {
                    terrainCollision[k].Add(currentLine);
                }
            }
        }

        // Tests collision against lander. Returns state of collision between lander and terrain.
        public CollisionState TestTerrainCollision(CircleCollider collider)
        {
            // Broadphase - Find which partions the circle collider intersects.
            List<int> partitionIndices = new List<int>();

            // Find extremities of collider.
            int leftIndex = (int)((collider.position.X - collider.radius) / partitionLength);
            int rightIndex = (int)((collider.position.X + collider.radius) / partitionLength);

            // Get all indices between the extremeties.
            for (int i = leftIndex; i <= rightIndex; i++)
            {
                partitionIndices.Add(i);
            }

            // Narrowphase - Test which lines are intersecting with circle collider.
            for (int i = 0; i < partitionIndices.Count; i++)
            {
                // Grab all lines within current partition, then do circle collider.
                for (int k = 0; k < terrainCollision[partitionIndices[i]].Count; k++)
                {
                    LineEntry currentLine = terrainCollision[partitionIndices[i]][k];
                    Vector2 v1 = new Vector2(currentLine.rightVertex.vertex.Position.X - currentLine.leftVertex.vertex.Position.X, currentLine.rightVertex.vertex.Position.Y - currentLine.leftVertex.vertex.Position.Y);
                    Vector2 v2 = new Vector2(currentLine.leftVertex.vertex.Position.X - collider.position.X, currentLine.leftVertex.vertex.Position.Y - collider.position.Y);

                    float b = -2 * (v1.X * v2.X + v1.Y * v2.Y);
                    float c = 2 * (v1.X * v1.X + v1.Y * v1.Y);
                    double d = (float)(Math.Sqrt(b * b - 2 * c * (v2.X * v2.X + v2.Y * v2.Y - collider.radius * collider.radius)));

                    if (!double.IsNaN(d))
                    {
                        float u1 = (float)(b - d) / c;
                        float u2 = (float)(b + d) / c;

                        if (u1 <= 1 && u1 >= 0)
                        {
                            // If point on the line segment
                            if (currentLine.isSafeZone)
                            {
                                return CollisionState.SAFE_COLLIDE;
                            }
                            else
                            {
                                return CollisionState.DANGEROUS_COLLIDE;
                            }
                        }

                        if (u2 <= 1 && u2 >= 0)
                        {
                            // If point on the line segment
                            if (currentLine.isSafeZone)
                            {
                                return CollisionState.SAFE_COLLIDE;
                            }
                            else
                            {
                                return CollisionState.DANGEROUS_COLLIDE;
                            }
                        }
                    }
                }
            }

            return CollisionState.NO_COLLIDE;
        }
    }
}